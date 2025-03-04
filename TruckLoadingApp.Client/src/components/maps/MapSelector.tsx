import React, { useState, useCallback, useEffect } from 'react';
import { MapContainer, TileLayer, Marker, useMap, useMapEvents } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';

// Fix for default marker icons in Leaflet with webpack
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';

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
}

const defaultCenter = {
  lat: 0,
  lng: 0
};

// Component to handle map center updates
const MapUpdater: React.FC<{ center?: Location }> = ({ center }) => {
  const map = useMap();
  
  useEffect(() => {
    if (center) {
      map.setView([center.lat, center.lng], 15);
    }
  }, [center, map]);

  return null;
};

// Component to handle map clicks
const MapClickHandler: React.FC<{ onMapClick: (e: L.LeafletMouseEvent) => void }> = ({ onMapClick }) => {
  useMapEvents({
    click: onMapClick,
  });

  return null;
};

const MapSelector: React.FC<MapSelectorProps> = ({
  initialLocation,
  onLocationSelect,
  placeholder = 'Search location...',
  className = ''
}) => {
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedLocation, setSelectedLocation] = useState<Location | undefined>(initialLocation);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (initialLocation) {
      setSelectedLocation(initialLocation);
    }
  }, [initialLocation]);

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

  const handleMapClick = useCallback(async (e: L.LeafletMouseEvent) => {
    const { lat, lng } = e.latlng;

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
    } catch (error) {
      console.error('Reverse geocoding error:', error);
      const location = { lat, lng };
      setSelectedLocation(location);
      onLocationSelect(location);
    }
  }, [onLocationSelect]);

  return (
    <div className={`w-full ${className}`}>
      <div className="mb-4">
        <div className="flex gap-2">
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
            placeholder={placeholder}
            className="flex-1 p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
          />
          <button
            onClick={handleSearch}
            disabled={loading}
            className="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
          >
            {loading ? 'Searching...' : 'Search'}
          </button>
        </div>
        {error && (
          <p className="mt-2 text-sm text-red-600">{error}</p>
        )}
      </div>

      <div className="h-[400px] w-full rounded-lg overflow-hidden border border-gray-300">
        <MapContainer
          center={selectedLocation || defaultCenter}
          zoom={selectedLocation ? 15 : 2}
          style={{ width: '100%', height: '100%' }}
        >
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          <MapUpdater center={selectedLocation} />
          <MapClickHandler onMapClick={handleMapClick} />
          {selectedLocation && (
            <Marker position={[selectedLocation.lat, selectedLocation.lng]} />
          )}
        </MapContainer>
      </div>

      {selectedLocation?.address && (
        <div className="mt-2 text-sm text-gray-600">
          Selected: {selectedLocation.address}
        </div>
      )}
    </div>
  );
};

export default MapSelector; 