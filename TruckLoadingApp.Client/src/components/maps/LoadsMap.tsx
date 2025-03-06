import React, { useState } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import { AvailableLoad } from '../../services/trucker.service';
import { Link } from 'react-router-dom';
import { FaTruck, FaWeightHanging, FaCalendarAlt } from 'react-icons/fa';
import { format } from 'date-fns';

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

// Create a custom truck icon
const truckIcon = new L.Icon({
  iconUrl: 'https://cdn-icons-png.flaticon.com/512/3774/3774278.png',
  iconSize: [32, 32],
  iconAnchor: [16, 16],
  popupAnchor: [0, -16]
});

interface LoadsMapProps {
  loads: AvailableLoad[];
  currentLocation: { latitude: number; longitude: number } | null;
}

// Component to handle map center updates
const MapUpdater: React.FC<{ center: { lat: number; lng: number } }> = ({ center }) => {
  const map = useMap();
  
  React.useEffect(() => {
    map.setView([center.lat, center.lng], map.getZoom());
  }, [center, map]);

  return null;
};

const formatDate = (dateString: string | Date) => {
  if (!dateString) return 'N/A';
  try {
    return format(new Date(dateString), 'MMM dd, yyyy');
  } catch (e) {
    return 'Invalid date';
  }
};

const LoadsMap: React.FC<LoadsMapProps> = ({ loads, currentLocation }) => {
  const [selectedLoad, setSelectedLoad] = useState<AvailableLoad | null>(null);

  if (!currentLocation) {
    return (
      <div className="flex justify-center items-center h-96 bg-gray-100 rounded-lg">
        <p className="text-gray-500">Loading location data...</p>
      </div>
    );
  }

  return (
    <div className="h-96 rounded-lg overflow-hidden shadow-md">
      <MapContainer 
        center={[currentLocation.latitude, currentLocation.longitude]} 
        zoom={10} 
        style={{ height: '100%', width: '100%' }}
      >
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        
        {/* Current location marker */}
        <Marker 
          position={[currentLocation.latitude, currentLocation.longitude]}
          icon={new L.Icon({
            iconUrl: 'https://cdn-icons-png.flaticon.com/512/684/684908.png',
            iconSize: [32, 32],
            iconAnchor: [16, 16],
            popupAnchor: [0, -16]
          })}
        >
          <Popup>
            <div className="text-center">
              <strong>Your Location</strong>
            </div>
          </Popup>
        </Marker>

        {/* Load markers */}
        {loads.map(load => {
          if (!load.pickupLocation?.latitude || !load.pickupLocation?.longitude) return null;
          
          return (
            <Marker
              key={load.id}
              position={[Number(load.pickupLocation.latitude), Number(load.pickupLocation.longitude)]}
              icon={truckIcon}
              eventHandlers={{
                click: () => {
                  setSelectedLoad(load);
                }
              }}
            >
              <Popup>
                <div className="p-2 max-w-xs">
                  <div className="flex items-center mb-2">
                    <FaTruck className="text-indigo-600 mr-2" />
                    <h3 className="font-semibold text-indigo-600">Load #{load.id}</h3>
                  </div>
                  
                  <div className="text-sm mb-1">
                    <strong>From:</strong> {load.pickupLocation?.address || 'Unknown'}
                  </div>
                  
                  <div className="text-sm mb-1">
                    <strong>To:</strong> {load.deliveryLocation?.address || 'Unknown'}
                  </div>
                  
                  <div className="flex items-center text-sm mb-1">
                    <FaCalendarAlt className="text-gray-500 mr-1" />
                    <span>Pickup: {formatDate(load.pickupDate)}</span>
                  </div>
                  
                  <div className="flex items-center text-sm mb-2">
                    <FaWeightHanging className="text-gray-500 mr-1" />
                    <span>Weight: {load.weight} kg</span>
                  </div>
                  
                  <div className="text-sm mb-2">
                    <strong>Distance:</strong> {Math.round(load.distance || 0)} km
                  </div>
                  
                  <Link
                    to={`/loads/${load.id}/details`}
                    className="block w-full text-center bg-indigo-600 hover:bg-indigo-700 text-white text-sm py-1 px-2 rounded"
                  >
                    View Details
                  </Link>
                </div>
              </Popup>
            </Marker>
          );
        })}
        
        <MapUpdater center={{ lat: currentLocation.latitude, lng: currentLocation.longitude }} />
      </MapContainer>
    </div>
  );
};

export default LoadsMap; 