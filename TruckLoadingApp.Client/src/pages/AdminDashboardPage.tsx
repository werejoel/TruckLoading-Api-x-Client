import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import AdminDashboard from '../components/admin/AdminDashboard';

const AdminDashboardPage: React.FC = () => {
  const { isAuthenticated, hasRole, loading } = useAuth();

  // Show loading state
  if (loading) {
    return (
      <div className="flex justify-center items-center h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
      </div>
    );
  }

  // Redirect if not authenticated or not an admin
  if (!isAuthenticated || !hasRole('Admin')) {
    return <Navigate to="/unauthorized" />;
  }

  return <AdminDashboard />;
};

export default AdminDashboardPage; 