import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import Navbar from '../components/layout/Navbar';
import { FaTruck, FaList, FaRoute } from 'react-icons/fa';

const TruckerDashboardPage: React.FC = () => {
  const { user } = useAuth();

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Welcome, {user?.firstName || 'Trucker'}!</h1>
          <p className="mt-2 text-gray-600">Manage your deliveries and view available loads.</p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {/* Available Loads Card */}
          <Link
            to="/available-loads"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200"
          >
            <div className="flex items-center mb-4">
              <FaList className="text-blue-500 text-2xl mr-3" />
              <h2 className="text-xl font-semibold text-gray-900">Available Loads</h2>
            </div>
            <p className="text-gray-600">Browse and accept available load requests.</p>
          </Link>

          {/* My Deliveries Card */}
          <Link
            to="/my-deliveries"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200"
          >
            <div className="flex items-center mb-4">
              <FaTruck className="text-green-500 text-2xl mr-3" />
              <h2 className="text-xl font-semibold text-gray-900">My Deliveries</h2>
            </div>
            <p className="text-gray-600">View and manage your current deliveries.</p>
          </Link>

          {/* Route Planning Card */}
          <Link
            to="/route-planning"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200"
          >
            <div className="flex items-center mb-4">
              <FaRoute className="text-orange-500 text-2xl mr-3" />
              <h2 className="text-xl font-semibold text-gray-900">Route Planning</h2>
            </div>
            <p className="text-gray-600">Plan and optimize your delivery routes.</p>
          </Link>
        </div>

        {/* Recent Activity Section */}
        <div className="mt-12">
          <h2 className="text-2xl font-bold text-gray-900 mb-6">Recent Activity</h2>
          <div className="bg-white rounded-lg shadow-md overflow-hidden">
            <div className="divide-y divide-gray-200">
              {/* We'll add actual activity items later */}
              <p className="p-4 text-gray-600 text-center">No recent activity to display.</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TruckerDashboardPage; 