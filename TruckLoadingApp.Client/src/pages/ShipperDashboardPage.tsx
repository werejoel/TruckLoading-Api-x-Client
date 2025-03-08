import React, { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import "../css/ShiperDashboard.css";
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

const ShipperDashboardPage: React.FC = () => {
  const { user, logout } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();
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
            <h1 className="page-title text-center">Welcome, {user?.firstName || 'Shipper'}!</h1>
            <p className="page-description text-center">Manage your loads and shipments from your dashboard.</p>
          </div>

          <div className="dashboard-grid">
            {/* Create New Load Card */}
            <Link
              to="/loads/create"
              className="dashboard-card create-card"
            >
              <div className="card-header">
                <div className="card-icon">
                  <FaPlus />
                </div>
                <h2 className="card-title">Create New Load</h2>
              </div>
              <p className="card-description">Create a new load request for transportation.</p>
            </Link>

            {/* View Loads Card */}
            <Link
              to="/loads"
              className="dashboard-card view-card"
            >
              <div className="card-header">
                <div className="card-icon">
                  <FaList />
                </div>
                <h2 className="card-title">View Loads</h2>
              </div>
              <p className="card-description">View and manage your existing loads.</p>
            </Link>

            {/* Find Trucks Card */}
            <Link
              to="/available-trucks"
              className="dashboard-card find-card"
            >
              <div className="card-header">
                <div className="card-icon">
                  <FaTruck />
                </div>
                <h2 className="card-title">Find Trucks</h2>
              </div>
              <p className="card-description">Search for available trucks for your loads.</p>
            </Link>
          </div>

          {/* Recent Activity Section */}
          <div className="activity-section">
            <div className="activity-header">
              <h2 className="activity-title">Recent Activity</h2>
            </div>
            <div className="activity-card">
              <div className="activity-empty">
                <p>No recent activity to display.</p>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
};

export default ShipperDashboardPage;