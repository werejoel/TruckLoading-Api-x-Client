import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import Navbar from '../components/layout/Navbar';
import { FaTruck, FaList, FaRoute, FaPlus, FaExclamationTriangle } from 'react-icons/fa';
import truckService from '../services/truck.service';
import truckerService from '../services/trucker.service';
import { Truck } from '../types/truck.types';
import { Booking } from '../services/shipper.service';
import { format } from 'date-fns';
import '../css/TruckerDashboard.css';

//Main Function 
const TruckerDashboardPage: React.FC = () => {
  const { user } = useAuth();
  const [trucks, setTrucks] = useState<Truck[]>([]);
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  //useEffect Function
  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);

        // Fetch trucks
        const trucksData = await truckService.getCompanyTrucks();
        setTrucks(trucksData);

        // Fetch bookings
        const bookingsData = await truckerService.getBookings();
        setBookings(bookingsData);
      }
      catch (err: any) {
        console.error('Error fetching trucker data:', err);
        setError(err.message || 'Failed to load data');
      }
      finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const getStatusBadgeColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'available':
        return 'badge badge-green';
      case 'in transit':
      case 'intransit':
        return 'badge badge-blue';
      case 'maintenance':
        return 'badge badge-yellow';
      case 'out of service':
      case 'outofservice':
        return 'badge badge-red';
      default:
        return 'badge badge-gray';
    }
  };

  const getBookingStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return 'badge badge-yellow';
      case 'accepted':
        return 'badge badge-green';
      case 'in transit':
      case 'intransit':
        return 'badge badge-blue';
      case 'completed':
        return 'badge badge-indigo';
      case 'cancelled':
        return 'badge badge-red';
      default:
        return 'badge badge-gray';
    }
  };

  const formatDate = (dateString: string | Date) => {
    if (!dateString) return 'N/A';
    try {
      return format(new Date(dateString), 'MMM dd, yyyy');
    } catch (e) {
      return 'Invalid date';
    }
  };

  return (
    <div className="dashboard-container">
      <Navbar />
      <div className="dashboard-content">
        <div className="dashboard-header">
          <h1 className="dashboard-title">Welcome, {user?.firstName || 'Trucker'}!</h1>
          <p className="dashboard-subtitle">Manage your deliveries and view available loads.</p>
        </div>

        {/* Error message */}
        {error && (
          <div className="error-container">
            <div className="error-content">
              <FaExclamationTriangle className="error-icon" />
              <div className="error-message">{error}</div>
            </div>
          </div>
        )}

        {/* Quick Actions */}
        <div className="actions-grid">
          {/* Available Loads Card */}
          <Link
            to="/available-loads"
            className="action-card"
          >
            <div className="action-header">
              <FaList className="action-icon blue" />
              <h2 className="action-title">Available Loads</h2>
            </div>
            <p className="action-description">Browse and accept available load requests.</p>
          </Link>

          {/* My Deliveries Card */}
          <Link
            to="/my-deliveries"
            className="action-card"
          >
            <div className="action-header">
              <FaTruck className="action-icon green" />
              <h2 className="action-title">My Deliveries</h2>
            </div>
            <p className="action-description">View and manage your current deliveries.</p>
          </Link>

          {/* Register Truck Card */}
          <Link
            to="/trucker/register-truck"
            className="action-card"
          >
            <div className="action-header">
              <FaPlus className="action-icon purple" />
              <h2 className="action-title">Register Truck</h2>
            </div>
            <p className="action-description">Add a new truck to your fleet.</p>
          </Link>
        </div>

        {/* My Trucks Section */}
        <div className="section-header">
          <h2 className="section-title">My Trucks</h2>
          <Link
            to="/trucker/register-truck"
            className="btn btn-primary"
          >
            <FaPlus className="btn-icon" />
            Add Truck
          </Link>
        </div>

        {loading ? (
          <div className="skeleton">
            <div className="skeleton-animate">
              <div className="skeleton-line skeleton-line-sm"></div>
              <div className="skeleton-line skeleton-line-md"></div>
              <div className="skeleton-line skeleton-line-lg"></div>
            </div>
          </div>
        ) : trucks.length === 0 ? (
          <div className="empty-state">
            <FaTruck className="empty-icon" />
            <h3 className="empty-title">No trucks registered</h3>
            <p className="empty-description">
              Get started by registering your first truck.
            </p>
            <div className="empty-action">
              <Link
                to="/trucker/register-truck"
                className="btn btn-primary"
              >
                <FaPlus className="btn-icon" />
                Register Truck
              </Link>
            </div>
          </div>
        ) : (
          <div className="list-container">
            <ul className="list">
              {trucks.map((truck) => (
                <li key={truck.id} className="list-item">
                  <div className="list-item-content">
                    <div className="list-item-header">
                      <div className="list-item-title">
                        <FaTruck className="list-item-icon" />
                        <p className="list-item-title-text">
                          {truck.numberPlate}
                        </p>
                      </div>
                      <div>
                        <span className={getStatusBadgeColor(truck.operationalStatus)}>
                          {truck.operationalStatus.toUpperCase()}
                        </span>
                      </div>
                    </div>
                    <div className="list-item-details">
                      <div className="list-item-info">
                        <p className="list-item-info-text">
                          {truck.truckType?.name || `Type ID: ${truck.truckTypeId}`}
                        </p>
                        <p className="list-item-info-text">
                          Capacity: {truck.loadCapacityWeight} kg
                        </p>
                      </div>
                      <div>
                        <Link
                          to={`/trucker/trucks/${truck.id}`}
                          className="list-item-link"
                        >
                          View Details
                        </Link>
                      </div>
                    </div>
                  </div>
                </li>
              ))}
            </ul>
          </div>
        )}

        {/* Recent Bookings Section */}
        <div className="section-header">
          <h2 className="section-title">Recent Bookings</h2>
        </div>
        {loading ? (
          <div className="skeleton">
            <div className="skeleton-animate">
              <div className="skeleton-line skeleton-line-sm"></div>
              <div className="skeleton-line skeleton-line-md"></div>
              <div className="skeleton-line skeleton-line-lg"></div>
            </div>
          </div>
        ) : bookings.length === 0 ? (
          <div className="empty-state">
            <p className="empty-description">No recent bookings to display.</p>
            <Link
              to="/available-loads"
              className="btn btn-primary"
            >
              Find Available Loads
            </Link>
          </div>
        ) : (
          <div className="list-container">
            <ul className="list">
              {bookings.slice(0, 5).map((booking) => (
                <li key={booking.id} className="list-item">
                  <div className="list-item-content">
                    <div className="list-item-header">
                      <div className="list-item-title">
                        <p className="list-item-title-text">
                          Booking #{booking.id}
                        </p>
                      </div>
                      <div>
                        <span className={getBookingStatusColor(booking.status)}>
                          {booking.status.toUpperCase()}
                        </span>
                      </div>
                    </div>
                    <div className="list-item-details">
                      <div className="list-item-info">
                        <p className="list-item-info-text">
                          Load: {booking.load?.description || `#${booking.loadId}`}
                        </p>
                        <p className="list-item-info-text">
                          Price: {booking.agreedPrice} {booking.currency}
                        </p>
                      </div>
                      <div>
                        <Link
                          to={`/my-deliveries/${booking.id}`}
                          className="list-item-link"
                        >
                          View Details
                        </Link>
                      </div>
                    </div>
                  </div>
                </li>
              ))}
            </ul>
            {bookings.length > 5 && (
              <div className="list-footer">
                <Link
                  to="/my-deliveries"
                  className="view-all-link"
                >
                  View all bookings
                </Link>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default TruckerDashboardPage;