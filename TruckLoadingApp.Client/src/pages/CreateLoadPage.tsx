import React, { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import LoadFormWizard from '../components/forms/LoadFormWizard';
import { LoadCreateRequest } from '../services/shipper.service';
import shipperService from '../services/shipper.service';
import { 
  FaTruck, 
  FaPlus, 
  FaList, 
  FaHome, 
  FaUser, 
  FaChartLine, 
  FaCog, 
  FaSignOutAlt,
  FaBars,
  FaShippingFast
} from 'react-icons/fa';

const CreateLoadPage: React.FC = () => {
  const { user, logout } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);
  const [sidebarOpen, setSidebarOpen] = useState(true);

  const toggleSidebar = () => {
    setSidebarOpen(!sidebarOpen);
  };

  // Function to get user initials for avatar
  const getUserInitials = () => {
    if (user?.firstName && user?.lastName) {
      return `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`;
    }
    return 'SH';
  };

  // Handle logout function
  const handleLogout = async (e: React.MouseEvent) => {
    e.preventDefault();
    try {
      await logout();
      navigate('/login');
    } catch (error) {
      console.error("Logout failed:", error);
    }
  };

  const handleSuccess = async (loadData: LoadCreateRequest) => {
    try {
      const createdLoad = await shipperService.createLoad(loadData);
      navigate('/shipper/dashboard', {
        state: { message: 'Load created successfully!' }
      });
    } catch (error: any) {
      console.error('Error creating load:', error);
      setError(error.response?.data?.message || 'Failed to create load. Please try again.');
    }
  };

  const handleCancel = () => {
    navigate('/shipper/dashboard');
  };

  return (
    <div className="app-container">
      {/* Mobile Menu Toggle */}
      <button className="menu-toggle" onClick={toggleSidebar}>
        <FaBars />
      </button>

      {/* Sidebar */}
      <aside className={`sidebar ${sidebarOpen ? 'open' : ''}`}>
        <div className="sidebar-header">
          <Link to="/dashboard" className="sidebar-logo">
            <FaShippingFast />
            <span>Shipper</span>
          </Link>
        </div>

        <nav className="sidebar-menu">
          <Link to="/dashboard" className={`menu-item ${location.pathname === '/dashboard' ? 'active' : ''}`}>
            <FaHome />
            <span>Dashboard</span>
          </Link>
          <Link to="/loads/create" className={`menu-item ${location.pathname === '/loads/create' ? 'active' : ''}`}>
            <FaPlus />
            <span>Create Load</span>
          </Link>
          <Link to="/loads" className={`menu-item ${location.pathname === '/loads' ? 'active' : ''}`}>
            <FaList />
            <span>My Loads</span>
          </Link>
          <Link to="/available-trucks" className={`menu-item ${location.pathname === '/available-trucks' ? 'active' : ''}`}>
            <FaTruck />
            <span>Find Trucks</span>
          </Link>
          <Link to="/analytics" className={`menu-item ${location.pathname === '/analytics' ? 'active' : ''}`}>
            <FaChartLine />
            <span>Analytics</span>
          </Link>
          <Link to="/profile" className={`menu-item ${location.pathname === '/profile' ? 'active' : ''}`}>
            <FaUser />
            <span>Profile</span>
          </Link>
          <Link to="/settings" className={`menu-item ${location.pathname === '/settings' ? 'active' : ''}`}>
            <FaCog />
            <span>Settings</span>
          </Link>
        </nav>

        <div className="sidebar-footer">
          <div className="user-info">
            <div className="user-avatar">
              {getUserInitials()}
            </div>
            <div className="user-details">
              <div className="user-name">{user?.firstName} {user?.lastName || 'Shipper'}</div>
              <div className="user-role">Shipper Account</div>
            </div>
          </div>
          <a href="#" onClick={handleLogout} className="menu-item" style={{ marginTop: '1rem' }}>
            <FaSignOutAlt />
            <span>Logout</span>
          </a>
        </div>
      </aside>

      {/* Main Content */}
      <main className="main-content">
        <div className="content-wrapper">
          <div className="page-header">
            <h1 className="page-title text-center">Create New Load</h1>
            <p className="page-description text-center">Please fill in the load details using the form below.</p>
          </div>

          {error && (
            <div className="mb-4 rounded-md bg-red-50 p-4">
              <div className="flex">
                <div className="ml-3">
                  <h3 className="text-sm font-medium text-red-800">{error}</h3>
                </div>
              </div>
            </div>
          )}

          <LoadFormWizard
            onSuccess={handleSuccess}
            onCancel={handleCancel}
          />
        </div>
      </main>
    </div>
  );
};

export default CreateLoadPage;