import React, { useState, useCallback, useEffect, useRef } from 'react';
import { MapContainer, TileLayer, Marker, useMap, useMapEvents, Popup } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import 'leaflet-routing-machine';

// Fix for default marker icons in Leaflet with webpack
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';
import { FaLocationArrow, FaSpinner, FaRoute, FaSearch } from 'react-icons/fa';

const DefaultIcon = L.icon({
  iconUrl: icon,
  shadowUrl: iconShadow,
  iconSize: [25, 41],
  iconAnchor: [12, 41]
});

L.Marker.prototype.options.icon = DefaultIcon;

interface Location {
  lat: number;
  lng: number;
  address?: string;
}

interface MapSelectorProps {
  initialLocation?: Location;
  onLocationSelect: (location: Location) => void;
  placeholder?: string;
  className?: string;
  secondLocation?: Location; // Optional second location for route visualization
  isPickup?: boolean; // Used to identify if this is a pickup location selector
}

// Default center - will be replaced by user's location if available
const defaultCenter = {
  lat: 0,
  lng: 0
};

// Component to handle map center updates
const MapUpdater: React.FC<{ center?: Location; userLocation?: Location }> = ({ center, userLocation }) => {
  const map = useMap();
  const firstLoad = useRef(true);
  
  useEffect(() => {
    if (center) {
      map.setView([center.lat, center.lng], 15);
    } else if (userLocation && firstLoad.current) {
      map.setView([userLocation.lat, userLocation.lng], 13);
      firstLoad.current = false;
    }
  }, [center, map, userLocation]);

  return null;
};

// Component to handle map clicks
const MapClickHandler: React.FC<{ onMapClick: (e: L.LeafletMouseEvent) => void }> = ({ onMapClick }) => {
  useMapEvents({
    click: onMapClick,
  });

  return null;
};

// Component to render route between points
const RouteDisplay: React.FC<{ from: Location; to: Location }> = ({ from, to }) => {
  const map = useMap();
  
  useEffect(() => {
    if (!from || !to) return;
    
    // Cast the routing control to any to bypass the TypeScript error
    // with the createMarker property
    const routingControl = (L.Routing.control as any)({
      waypoints: [
        L.latLng(from.lat, from.lng),
        L.latLng(to.lat, to.lng)
      ],
      routeWhileDragging: true,
      showAlternatives: false,
      fitSelectedRoutes: true,
      lineOptions: {
        styles: [{ color: '#6366F1', opacity: 0.8, weight: 6 }],
        extendToWaypoints: true,
        missingRouteTolerance: 0
      },
      createMarker: () => null, // Don't create markers, we'll handle that
      show: false // Don't show the itinerary panel
    }).addTo(map);
    
    return () => {
      map.removeControl(routingControl);
    };
  }, [map, from, to]);
  
  return null;
};

// Component for autocomplete suggestions
const AutocompleteSuggestions: React.FC<{
  suggestions: any[];
  loading: boolean;
  onSelect: (suggestion: any) => void;
  visible: boolean;
}> = ({ suggestions, loading, onSelect, visible }) => {
  if (!visible) return null;
  
  return (
    <div className="absolute z-[1000] w-full bg-white mt-1 rounded-md shadow-lg max-h-60 overflow-y-auto">
      {loading ? (
        <div className="p-4 text-center text-gray-500 flex items-center justify-center">
          <FaSpinner className="animate-spin h-5 w-5 mr-2" />
          <span>Searching...</span>
        </div>
      ) : suggestions.length > 0 ? (
        <ul className="py-1">
          {suggestions.map((suggestion, index) => (
            <li 
              key={index}
              onClick={() => onSelect(suggestion)}
              className="px-4 py-2 hover:bg-gray-100 cursor-pointer text-sm"
            >
              {suggestion.display_name}
            </li>
          ))}
        </ul>
      ) : (
        <div className="p-4 text-center text-gray-500">
          No results found
        </div>
      )}
    </div>
  );
};

const MapSelector: React.FC<MapSelectorProps> = ({
  initialLocation,
  onLocationSelect,
  placeholder = 'Search location...',
  className = '',
  secondLocation,
  isPickup
}) => {
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedLocation, setSelectedLocation] = useState<Location | undefined>(initialLocation);
  const [userLocation, setUserLocation] = useState<Location | undefined>();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [suggestions, setSuggestions] = useState<any[]>([]);
  const [showSuggestions, setShowSuggestions] = useState(false);
  const [isLocatingUser, setIsLocatingUser] = useState(false);
  const searchInputRef = useRef<HTMLInputElement>(null);

  // Get user's location on component mount
  useEffect(() => {
    const getUserLocation = () => {
      // This will trigger the browser's location permission prompt
      if (navigator.geolocation) {
        setIsLocatingUser(true);
        navigator.geolocation.getCurrentPosition(
          (position) => {
            const { latitude, longitude } = position.coords;
            const userLoc = { lat: latitude, lng: longitude };
            setUserLocation(userLoc);
            
            // If this is a pickup location selector and no location is selected yet,
            // ask the user if they want to use their current location
            if (isPickup && !selectedLocation && !initialLocation) {
              const confirmUse = window.confirm('Do you want to use your current location as the pickup location?');
              if (confirmUse) {
                // Get address for the location
                fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${latitude}&lon=${longitude}`)
                  .then(response => response.json())
                  .then(data => {
                    const locationWithAddress = {
                      ...userLoc,
                      address: data.display_name
                    };
                    setSelectedLocation(locationWithAddress);
                    onLocationSelect(locationWithAddress);
                    setSearchQuery(data.display_name);
                  })
                  .catch(err => {
                    console.error('Error getting address:', err);
                    setSelectedLocation(userLoc);
                    onLocationSelect(userLoc);
                  });
              }
            }
            setIsLocatingUser(false);
          },
          (error) => {
            console.error('Error getting user location:', error);
            setIsLocatingUser(false);
          },
          { enableHighAccuracy: true, timeout: 5000, maximumAge: 0 }
        );
      }
    };

    getUserLocation();
  }, [isPickup, initialLocation, selectedLocation, onLocationSelect]);

  useEffect(() => {
    if (initialLocation) {
      setSelectedLocation(initialLocation);
    }
  }, [initialLocation]);

  // Handle search input changes with debounce for autocomplete
  useEffect(() => {
    if (!searchQuery.trim() || searchQuery.length < 3) {
      setSuggestions([]);
      return;
    }

    const timer = setTimeout(() => {
      setLoading(true);
      fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(searchQuery)}&limit=5`)
        .then(response => response.json())
        .then(data => {
          setSuggestions(data);
          setShowSuggestions(true);
        })
        .catch(error => {
          console.error('Geocoding error:', error);
          setError('Error searching for location');
        })
        .finally(() => setLoading(false));
    }, 300);

    return () => clearTimeout(timer);
  }, [searchQuery]);

  // Handle clicking outside of suggestions to close them
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (searchInputRef.current && !searchInputRef.current.contains(event.target as Node)) {
        setShowSuggestions(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleSearch = useCallback(async () => {
    if (!searchQuery.trim()) return;

    setLoading(true);
    setError(null);

    try {
      const response = await fetch(
        `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(searchQuery)}&limit=1`
      );
      const data = await response.json();

      if (data && data[0]) {
        const location = {
          lat: parseFloat(data[0].lat),
          lng: parseFloat(data[0].lon),
          address: data[0].display_name
        };
        setSelectedLocation(location);
        onLocationSelect(location);
        setShowSuggestions(false);
      } else {
        setError('No results found');
      }
    } catch (error) {
      console.error('Geocoding error:', error);
      setError('Error searching for location');
    } finally {
      setLoading(false);
    }
  }, [searchQuery, onLocationSelect]);

  const handleSuggestionSelect = (suggestion: any) => {
    const location = {
      lat: parseFloat(suggestion.lat),
      lng: parseFloat(suggestion.lon),
      address: suggestion.display_name
    };
    
    // Set the selected location
    setSelectedLocation(location);
    
    // Notify parent component about the selection
    onLocationSelect(location);
    
    // Update the search query with the selected address
    setSearchQuery(suggestion.display_name);
    
    // Hide the suggestions dropdown
    setShowSuggestions(false);
  };

  const handleMapClick = useCallback(async (e: L.LeafletMouseEvent) => {
    const { lat, lng } = e.latlng;
    setLoading(true);

    try {
      const response = await fetch(
        `https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}`
      );
      const data = await response.json();

      const location = {
        lat,
        lng,
        address: data.display_name
      };
      setSelectedLocation(location);
      onLocationSelect(location);
      setSearchQuery(data.display_name);
    } catch (error) {
      console.error('Reverse geocoding error:', error);
      const location = { lat, lng };
      setSelectedLocation(location);
      onLocationSelect(location);
    } finally {
      setLoading(false);
    }
  }, [onLocationSelect]);

  const handleUseMyLocation = () => {
    if (userLocation) {
      setIsLocatingUser(true);
      
      // Show confirmation dialog before using location as pickup
      if (isPickup) {
        const confirmUse = window.confirm('Do you want to use your current location as the pickup location?');
        if (!confirmUse) {
          setIsLocatingUser(false);
          return;
        }
      }
      
      fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${userLocation.lat}&lon=${userLocation.lng}`)
        .then(response => response.json())
        .then(data => {
          const locationWithAddress = {
            ...userLocation,
            address: data.display_name
          };
          setSelectedLocation(locationWithAddress);
          onLocationSelect(locationWithAddress);
          setSearchQuery(data.display_name);
        })
        .catch(err => {
          console.error('Error getting address:', err);
          setSelectedLocation(userLocation);
          onLocationSelect(userLocation);
        })
        .finally(() => setIsLocatingUser(false));
    }
  };

  return (
    <div className={`w-full ${className}`}>
      <div className="mb-4">
        <div className="relative">
          <div className="flex gap-2">
            <div className="relative flex-1">
              <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <FaSearch className="h-5 w-5 text-gray-400" />
              </div>
              <input
                ref={searchInputRef}
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                onFocus={() => searchQuery.length >= 3 && setShowSuggestions(true)}
                onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                placeholder={placeholder}
                className="flex-1 p-2 pl-10 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 w-full"
              />
              {loading && (
                <div className="absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none">
                  <FaSpinner className="animate-spin h-4 w-4 text-gray-400" />
                </div>
              )}
            </div>
            <button
              onClick={handleUseMyLocation}
              disabled={isLocatingUser}
              className="px-4 py-2 bg-gray-100 text-gray-700 rounded-md hover:bg-gray-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500 disabled:opacity-50 flex items-center"
              title="Use my current location"
            >
              {isLocatingUser ? (
                <FaSpinner className="animate-spin h-4 w-4" />
              ) : (
                <FaLocationArrow className="h-4 w-4" />
              )}
            </button>
            <button
              onClick={handleSearch}
              disabled={loading || !searchQuery.trim()}
              className="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
            >
              {loading ? 'Searching...' : 'Search'}
            </button>
          </div>
          <AutocompleteSuggestions 
            suggestions={suggestions}
            loading={loading}
            onSelect={handleSuggestionSelect}
            visible={showSuggestions}
          />
        </div>
        {error && (
          <p className="mt-2 text-sm text-red-600">{error}</p>
        )}
      </div>

      <div className="h-[400px] w-full rounded-lg overflow-hidden border border-gray-300 relative">
        {(loading || isLocatingUser) && (
          <div className="absolute z-10 bg-white bg-opacity-70 flex items-center justify-center w-full h-full">
            <div className="bg-white p-4 rounded-full shadow-md">
              <FaSpinner className="animate-spin h-10 w-10 text-indigo-600" />
            </div>
          </div>
        )}
        <MapContainer
          center={selectedLocation || userLocation || defaultCenter}
          zoom={selectedLocation ? 15 : userLocation ? 13 : 2}
          style={{ width: '100%', height: '100%' }}
        >
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          <MapUpdater center={selectedLocation} userLocation={userLocation} />
          <MapClickHandler onMapClick={handleMapClick} />
          
          {selectedLocation && (
            <Marker position={[selectedLocation.lat, selectedLocation.lng]}>
              {selectedLocation.address && (
                <Popup>
                  {isPickup ? 'Pickup: ' : 'Delivery: '}{selectedLocation.address}
                </Popup>
              )}
            </Marker>
          )}
          
          {userLocation && !selectedLocation && (
            <Marker position={[userLocation.lat, userLocation.lng]} opacity={0.7}>
              <Popup>Your location</Popup>
            </Marker>
          )}
          
          {selectedLocation && secondLocation && (
            <>
              <Marker position={[secondLocation.lat, secondLocation.lng]}>
                {secondLocation.address && (
                  <Popup>
                    {isPickup ? 'Delivery: ' : 'Pickup: '}{secondLocation.address}
                  </Popup>
                )}
              </Marker>
              <RouteDisplay from={isPickup ? selectedLocation : secondLocation} to={isPickup ? secondLocation : selectedLocation} />
            </>
          )}
        </MapContainer>
      </div>

      {selectedLocation?.address && (
        <div className="mt-2 text-sm text-gray-600 flex items-center">
          <span className="font-medium mr-2">Selected:</span> {selectedLocation.address}
        </div>
      )}
      
      {selectedLocation && secondLocation && (
        <div className="mt-2 p-2 bg-indigo-50 rounded-md text-sm text-indigo-700 flex items-center">
          <FaRoute className="mr-2" />
          <span>Route displayed between pickup and delivery locations</span>
        </div>
      )}
    </div>
  );
};

export default MapSelector;