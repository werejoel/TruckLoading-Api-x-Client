import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import Navbar from '../components/layout/Navbar';
import { FaTruck, FaPlus, FaList } from 'react-icons/fa';

const ShipperDashboardPage: React.FC = () => {
  const { user } = useAuth();

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Welcome, {user?.firstName || 'Shipper'}!</h1>
          <p className="mt-2 text-gray-600">Manage your loads and shipments from your dashboard.</p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {/* Create New Load Card */}
          <Link
            to="/loads/create"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200"
          >
            <div className="flex items-center mb-4">
              <FaPlus className="text-blue-500 text-2xl mr-3" />
              <h2 className="text-xl font-semibold text-gray-900">Create New Load</h2>
            </div>
            <p className="text-gray-600">Create a new load request for transportation.</p>
          </Link>

          {/* View Loads Card */}
          <Link
            to="/loads"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200"
          >
            <div className="flex items-center mb-4">
              <FaList className="text-green-500 text-2xl mr-3" />
              <h2 className="text-xl font-semibold text-gray-900">View Loads</h2>
            </div>
            <p className="text-gray-600">View and manage your existing loads.</p>
          </Link>

          {/* Find Trucks Card */}
          <Link
            to="/available-trucks"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200"
          >
            <div className="flex items-center mb-4">
              <FaTruck className="text-orange-500 text-2xl mr-3" />
              <h2 className="text-xl font-semibold text-gray-900">Find Trucks</h2>
            </div>
            <p className="text-gray-600">Search for available trucks for your loads.</p>
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

export default ShipperDashboardPage; 