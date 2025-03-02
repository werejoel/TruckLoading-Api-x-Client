import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import ProtectedRoute from './components/auth/ProtectedRoute';

// Pages
import LoginPage from './pages/LoginPage';
import ShipperRegisterPage from './pages/ShipperRegisterPage';
import TruckerRegisterPage from './pages/TruckerRegisterPage';
import CompanyRegisterPage from './pages/CompanyRegisterPage';
import DashboardPage from './pages/DashboardPage';
import CompanyDashboardPage from './pages/CompanyDashboardPage';
import DriverDetailPage from './pages/DriverDetailPage';
import TruckDetailPage from './pages/TruckDetailPage';
import UnauthorizedPage from './pages/UnauthorizedPage';
import NotFoundPage from './pages/NotFoundPage';
import AdminDashboardPage from './pages/AdminDashboardPage';

// Root redirect component
const RootRedirect = () => {
  const { isAuthenticated, hasRole, loading } = useAuth();
  
  if (loading) {
    return <div>Loading...</div>;
  }
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  
  if (hasRole('Admin')) {
    return <Navigate to="/admin/dashboard" replace />;
  } else if (hasRole('Company')) {
    return <Navigate to="/company/dashboard" replace />;
  } else if (hasRole('Trucker')) {
    return <Navigate to="/dashboard" replace />; // Can be changed to a trucker-specific dashboard later
  } else if (hasRole('Shipper')) {
    return <Navigate to="/shipper/dashboard" replace />; // Updated to redirect to shipper-specific dashboard
  } else {
    return <Navigate to="/dashboard" replace />;
  }
};

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          {/* Public routes */}
          <Route path="/" element={<RootRedirect />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register/shipper" element={<ShipperRegisterPage />} />
          <Route path="/register/trucker" element={<TruckerRegisterPage />} />
          <Route path="/register/company" element={<CompanyRegisterPage />} />
          <Route path="/unauthorized" element={<UnauthorizedPage />} />
          
          {/* Protected routes */}
          <Route element={<ProtectedRoute />}>
            <Route path="/dashboard" element={<DashboardPage />} />
          </Route>
          
          {/* Shipper-specific routes */}
          <Route element={<ProtectedRoute requiredRole="Shipper" />}>
            <Route path="/shipper/dashboard" element={<DashboardPage />} />
          </Route>
          
          {/* Trucker-specific routes */}
          <Route element={<ProtectedRoute requiredRole="Trucker" />}>
            {/* Add trucker-specific routes here */}
          </Route>
          
          {/* Company-specific routes */}
          <Route element={<ProtectedRoute requiredRole="Company" />}>
            <Route path="/company/dashboard" element={<CompanyDashboardPage />} />
            <Route path="/company/drivers/:id" element={<DriverDetailPage />} />
            <Route path="/company/trucks/:id" element={<TruckDetailPage />} />
          </Route>
          
          {/* Admin-specific routes */}
          <Route element={<ProtectedRoute requiredRole="Admin" />}>
            <Route path="/admin/dashboard" element={<AdminDashboardPage />} />
          </Route>
          
          {/* 404 route */}
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
