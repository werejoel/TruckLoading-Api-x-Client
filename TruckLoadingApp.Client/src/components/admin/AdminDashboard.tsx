import React, { useState } from 'react';
import UserManagement from './UserManagement';
import RoleManagement from './RoleManagement';
import ApiDebugInfo from './ApiDebugInfo';

const AdminDashboard: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'users' | 'roles' | 'api-debug'>('users');

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">Admin Dashboard</h1>
      
      {/* Tabs */}
      <div className="border-b border-gray-200 mb-6">
        <nav className="-mb-px flex space-x-8">
          <button
            onClick={() => setActiveTab('users')}
            className={`${
              activeTab === 'users'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
          >
            User Management
          </button>
          <button
            onClick={() => setActiveTab('roles')}
            className={`${
              activeTab === 'roles'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
          >
            Role Management
          </button>
          <button
            onClick={() => setActiveTab('api-debug')}
            className={`${
              activeTab === 'api-debug'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
          >
            API Debug
          </button>
        </nav>
      </div>

      {/* Tab Content */}
      <div className="mt-6">
        {activeTab === 'users' && <UserManagement />}
        {activeTab === 'roles' && <RoleManagement />}
        {activeTab === 'api-debug' && <ApiDebugInfo />}
      </div>
    </div>
  );
};

export default AdminDashboard; 