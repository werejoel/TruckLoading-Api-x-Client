import React, { useState } from 'react';
import PageTemplate from '../components/common/PageTemplate';
import { useAuth } from '../context/AuthContext';

const SearchTrucksPage: React.FC = () => {
  const { user } = useAuth();
  const [searchParams, setSearchParams] = useState({
    location: '',
    date: '',
    truckType: '',
    capacity: ''
  });

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    // TODO: Implement truck search functionality
  };

  return (
    <PageTemplate title="Search Available Trucks">
      <div className="max-w-3xl mx-auto">
        <form onSubmit={handleSearch} className="space-y-6">
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
            <div>
              <label htmlFor="location" className="block text-sm font-medium text-gray-700">
                Pickup Location
              </label>
              <input
                type="text"
                id="location"
                name="location"
                value={searchParams.location}
                onChange={(e) => setSearchParams({ ...searchParams, location: e.target.value })}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                placeholder="Enter city or address"
              />
            </div>

            <div>
              <label htmlFor="date" className="block text-sm font-medium text-gray-700">
                Pickup Date
              </label>
              <input
                type="date"
                id="date"
                name="date"
                value={searchParams.date}
                onChange={(e) => setSearchParams({ ...searchParams, date: e.target.value })}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
              />
            </div>

            <div>
              <label htmlFor="truckType" className="block text-sm font-medium text-gray-700">
                Truck Type
              </label>
              <select
                id="truckType"
                name="truckType"
                value={searchParams.truckType}
                onChange={(e) => setSearchParams({ ...searchParams, truckType: e.target.value })}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
              >
                <option value="">Select truck type</option>
                <option value="flatbed">Flatbed</option>
                <option value="box">Box Truck</option>
                <option value="refrigerated">Refrigerated</option>
                <option value="tanker">Tanker</option>
              </select>
            </div>

            <div>
              <label htmlFor="capacity" className="block text-sm font-medium text-gray-700">
                Minimum Capacity (tons)
              </label>
              <input
                type="number"
                id="capacity"
                name="capacity"
                value={searchParams.capacity}
                onChange={(e) => setSearchParams({ ...searchParams, capacity: e.target.value })}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                placeholder="Enter minimum capacity"
              />
            </div>
          </div>

          <div className="flex justify-end">
            <button
              type="submit"
              className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              Search Trucks
            </button>
          </div>
        </form>

        {/* Results section - to be implemented */}
        <div className="mt-8">
          <h2 className="text-lg font-medium text-gray-900 mb-4">Search Results</h2>
          <div className="bg-gray-50 p-4 rounded-md">
            <p className="text-gray-500 text-center">
              Enter search criteria and click "Search Trucks" to find available trucks
            </p>
          </div>
        </div>
      </div>
    </PageTemplate>
  );
};

export default SearchTrucksPage; 