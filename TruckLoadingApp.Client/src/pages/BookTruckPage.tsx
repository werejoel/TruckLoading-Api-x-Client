import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Layout from '../components/layout/Layout';
import BookingForm from '../components/booking/BookingForm';
import shipperService, { Load, TruckMatchResponse } from '../services/shipper.service';

const BookTruckPage: React.FC = () => {
  const navigate = useNavigate();
  const { loadId, truckId } = useParams<{ loadId: string; truckId: string }>();
  const [load, setLoad] = useState<Load | null>(null);
  const [truck, setTruck] = useState<TruckMatchResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        if (!loadId || !truckId) {
          throw new Error('Load ID and Truck ID are required');
        }
        
        // Fetch load data
        const loadData = await shipperService.getLoadById(parseInt(loadId));
        setLoad(loadData);
        
        // Fetch matching trucks for this load
        const trucks = await shipperService.getMatchingTrucksForLoad(parseInt(loadId), 500); // Large distance to ensure we find the truck
        const matchingTruck = trucks.find(t => t.id === parseInt(truckId));
        
        if (!matchingTruck) {
          throw new Error('Truck not found or not available for this load');
        }
        
        setTruck(matchingTruck);
      } catch (err: any) {
        console.error('Error fetching data:', err);
        setError(err.message || 'Failed to load data');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [loadId, truckId]);

  const handleSuccess = () => {
    navigate('/shipper/dashboard');
  };

  const handleCancel = () => {
    navigate(`/loads/${loadId}/matching-trucks`);
  };

  if (loading) {
    return (
      <Layout>
        <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
          <div className="px-4 py-6 sm:px-0 flex justify-center">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
          </div>
        </div>
      </Layout>
    );
  }

  if (error || !load || !truck) {
    return (
      <Layout>
        <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
          <div className="px-4 py-6 sm:px-0">
            <div className="bg-red-50 border-l-4 border-red-400 p-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <svg className="h-5 w-5 text-red-400" viewBox="0 0 20 20" fill="currentColor">
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                  </svg>
                </div>
                <div className="ml-3">
                  <p className="text-sm text-red-700">{error || 'Failed to load booking data'}</p>
                </div>
              </div>
            </div>
            <div className="mt-4">
              <button
                onClick={() => navigate('/shipper/dashboard')}
                className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                Back to Dashboard
              </button>
            </div>
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <BookingForm 
            load={load}
            truck={truck}
            onSuccess={handleSuccess} 
            onCancel={handleCancel}
          />
        </div>
      </div>
    </Layout>
  );
};

export default BookTruckPage; 