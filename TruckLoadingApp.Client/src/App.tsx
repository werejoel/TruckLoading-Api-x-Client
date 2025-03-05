import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import ProtectedRoute from './components/auth/ProtectedRoute';
import Layout from './components/layout/Layout';
import Landing from './pages/Landing';
import LoginPage from './pages/LoginPage';
import ShipperRegisterPage from './pages/ShipperRegisterPage';
import TruckerRegisterPage from './pages/TruckerRegisterPage';
import CompanyRegisterPage from './pages/CompanyRegisterPage';
import ShipperDashboardPage from './pages/ShipperDashboardPage';
import TruckerDashboardPage from './pages/TruckerDashboardPage';
import CompanyDashboardPage from './pages/CompanyDashboardPage';
import DriverDetailPage from './pages/DriverDetailPage';
import TruckDetailPage from './pages/TruckDetailPage';
import UnauthorizedPage from './pages/UnauthorizedPage';
import NotFoundPage from './pages/NotFoundPage';
import AdminDashboardPage from './pages/AdminDashboardPage';
import LoadsPage from './pages/loads/LoadsPage';
import CreateLoadPage from './pages/CreateLoadPage';
import EditLoadPage from './pages/EditLoadPage';
import LoadLocationPage from './pages/LoadLocationPage';
import MatchingTrucksPage from './pages/MatchingTrucksPage';
import BookTruckPage from './pages/BookTruckPage';
import RegisterTruckPage from './pages/trucker/RegisterTruckPage';
// import SearchTrucksPage from './pages/SearchTrucksPage';
// import BookingsPage from './pages/BookingsPage';
// import MyTrucksPage from './pages/MyTrucksPage';
// import AvailableLoadsPage from './pages/AvailableLoadsPage';
// import RoutePlanningPage from './pages/RoutePlanningPage';
// import DriversListPage from './pages/DriversListPage';
// import TrucksListPage from './pages/TrucksListPage';
// import CompanyAnalyticsPage from './pages/CompanyAnalyticsPage';
// import UsersManagementPage from './pages/UsersManagementPage';
// import CompaniesManagementPage from './pages/CompaniesManagementPage';
// import SystemSettingsPage from './pages/SystemSettingsPage';

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          {/* Public routes */}
          <Route path="/" element={<Landing />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register/shipper" element={<ShipperRegisterPage />} />
          <Route path="/register/trucker" element={<TruckerRegisterPage />} />
          <Route path="/register/company" element={<CompanyRegisterPage />} />
          <Route path="/unauthorized" element={<UnauthorizedPage />} />
          
          {/* Dashboard redirect */}
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute>
                <DashboardRedirect />
              </ProtectedRoute>
            }
          />
          
          {/* Shipper routes */}
          <Route element={<ProtectedRoute requiredRole="Shipper" />}>
            <Route path="/shipper/dashboard" element={<ShipperDashboardPage />} />
            <Route path="/loads" element={<LoadsPage />} />
            <Route path="/loads/create" element={<CreateLoadPage />} />
            <Route path="/loads/:id/select-location" element={<LoadLocationPage />} />
            <Route path="/loads/:id/matching-trucks" element={<MatchingTrucksPage />} />
            <Route path="/loads/edit/:id" element={<EditLoadPage />} />
            <Route path="/loads/:loadId/book/:truckId" element={<BookTruckPage />} />
          </Route>
          
          {/* Trucker routes */}
          <Route element={<ProtectedRoute requiredRole="Trucker" />}>
            <Route path="/trucker/dashboard" element={<TruckerDashboardPage />} />
            <Route path="/trucker/register-truck" element={<RegisterTruckPage />} />
          </Route>
          
          {/* Company routes */}
          <Route element={<ProtectedRoute requiredRole="Company" />}>
            <Route path="/company/dashboard" element={<CompanyDashboardPage />} />
            <Route path="/company/drivers/:id" element={<DriverDetailPage />} />
            <Route path="/company/trucks/:id" element={<TruckDetailPage />} />
          </Route>
          
          {/* Admin routes */}
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

// Component to redirect users to their role-specific dashboard
const DashboardRedirect: React.FC = () => {
  const { hasRole } = useAuth();

  if (hasRole('Shipper')) {
    return <Navigate to="/shipper/dashboard" replace />;
  }
  if (hasRole('Trucker')) {
    return <Navigate to="/trucker/dashboard" replace />;
  }
  if (hasRole('Company')) {
    return <Navigate to="/company/dashboard" replace />;
  }
  if (hasRole('Admin')) {
    return <Navigate to="/admin/dashboard" replace />;
  }

  return <Navigate to="/unauthorized" replace />;
};

export default App;
