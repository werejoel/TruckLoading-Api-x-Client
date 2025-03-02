import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

interface ProtectedRouteProps {
  requiredRole?: string;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ requiredRole }) => {
  const { isAuthenticated, loading, hasRole } = useAuth();

  // Show loading indicator while checking authentication
  if (loading) {
    return <div>Loading...</div>;
  }

  // If not authenticated, redirect to login
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  // If a specific role is required, check if the user has it
  if (requiredRole && !hasRole(requiredRole)) {
    return <Navigate to="/unauthorized" replace />;
  }

  // If authenticated and has the required role (if any), render the child routes
  return <Outlet />;
};

export default ProtectedRoute; 