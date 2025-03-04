import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { navigationConfig } from '../../config/navigation';

interface CompanyLayoutProps {
  children: React.ReactNode;
}

const CompanyLayout: React.FC<CompanyLayoutProps> = ({ children }) => {
  const { user } = useAuth();
  const location = useLocation();
  const companyNavItems = navigationConfig.company;

  // Company-specific stats
  const stats = [
    { name: 'Total Drivers', value: '12' },
    { name: 'Active Trucks', value: '8' },
    { name: 'Completed Deliveries', value: '143' },
    { name: 'Revenue', value: '$45,233' },
  ];

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Side Navigation */}
      <div className="fixed inset-y-0 left-0 w-64 bg-white shadow-lg">
        <div className="flex flex-col h-full">
          {/* Company Logo/Name */}
          <div className="flex items-center justify-center h-16 border-b">
            <Link to="/company/dashboard" className="text-xl font-bold text-indigo-600">
              {user?.companyName || 'Company Dashboard'}
            </Link>
          </div>

          {/* Navigation Items */}
          <nav className="flex-1 px-4 py-4 space-y-1 overflow-y-auto">
            {companyNavItems.map((item) => (
              <Link
                key={item.path}
                to={item.path}
                className={`flex items-center px-4 py-2 text-sm font-medium rounded-md ${
                  location.pathname === item.path
                    ? 'bg-indigo-50 text-indigo-600'
                    : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                }`}
              >
                {item.icon && (
                  <span className="mr-3">
                    <i className={item.icon}></i>
                  </span>
                )}
                {item.label}
              </Link>
            ))}
          </nav>

          {/* Company Stats */}
          <div className="p-4 border-t">
            <div className="grid grid-cols-2 gap-4">
              {stats.map((stat) => (
                <div key={stat.name} className="bg-gray-50 p-3 rounded-lg">
                  <dt className="text-xs font-medium text-gray-500 truncate">
                    {stat.name}
                  </dt>
                  <dd className="mt-1 text-sm font-semibold text-gray-900">
                    {stat.value}
                  </dd>
                </div>
              ))}
            </div>
          </div>

          {/* User Info */}
          <div className="p-4 border-t">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 rounded-full bg-indigo-600 flex items-center justify-center text-white">
                  {user?.firstName?.[0]}
                </div>
              </div>
              <div className="ml-3">
                <p className="text-sm font-medium text-gray-700">
                  {user?.firstName} {user?.lastName}
                </p>
                <p className="text-xs text-gray-500">{user?.companyName}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="pl-64">
        <main className="py-6">
          <div className="mx-auto px-4 sm:px-6 md:px-8">
            {children}
          </div>
        </main>
      </div>
    </div>
  );
};

export default CompanyLayout; 