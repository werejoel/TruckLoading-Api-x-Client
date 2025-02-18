var map;
var truckMarkers = {}; // Stores markers by truckId

window.initializeMap = () => {
    map = L.map('map').setView([0, 0], 2); // Default to world view

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors'
    }).addTo(map);
};

window.updateTruckLocation = (truckId, latitude, longitude) => {
    if (!map) {
        console.error("Map is not initialized yet!");
        return;
    }

    if (truckMarkers[truckId]) {
        truckMarkers[truckId].setLatLng([latitude, longitude]);
    } else {
        truckMarkers[truckId] = L.marker([latitude, longitude]).addTo(map)
            .bindPopup(`Truck ${truckId}`);
    }
};
