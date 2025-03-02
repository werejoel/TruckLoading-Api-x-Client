import React from 'react';
import { useAuth } from '../context/AuthContext';
import Layout from '../components/layout/Layout';
import ShipperDashboard from '../components/dashboard/ShipperDashboard';
import { UserType } from '../types/auth.types';

const DashboardPage: React.FC = () => {
  const { user, logout } = useAuth();

  // Render appropriate dashboard based on user type
  const renderDashboard = () => {
    if (!user) return null;

    switch (user.userType) {
      case UserType.Shipper:
        return <ShipperDashboard />;
      default:
        return (
          <div className="border-4 border-dashed border-gray-200 rounded-lg p-6">
            <h2 className="text-lg font-medium text-gray-900">Welcome, {user?.firstName || 'User'}!</h2>
            <p className="mt-2 text-gray-600">
              You are logged in as a {user?.userType || 'User'}.
            </p>
            <div className="mt-4">
              <h3 className="text-md font-medium text-gray-900">Your Account Information:</h3>
              <ul className="mt-2 list-disc list-inside text-gray-600">
                <li>Email: {user?.email || 'N/A'}</li>
                <li>Name: {user?.firstName} {user?.lastName}</li>
                <li>User Type: {user?.userType}</li>
                <li>Account Created: {user?.createdDate ? new Date(user.createdDate).toLocaleDateString() : 'N/A'}</li>
              </ul>
            </div>
            <div className="mt-6">
              <p className="text-gray-600">
                This is a placeholder dashboard. More features will be added soon.
              </p>
            </div>
          </div>
        );
    }
  };

  return (
    <Layout>
      <div className="bg-white shadow">
        <div className="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8 flex justify-between items-center">
          <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
          <button
            onClick={logout}
            className="px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            Logout
          </button>
        </div>
      </div>
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          {renderDashboard()}
        </div>
      </div>
    </Layout>
  );
};

export default DashboardPage; 