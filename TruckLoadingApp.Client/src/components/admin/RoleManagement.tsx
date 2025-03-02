import React, { useState, useEffect } from 'react';
import adminService, { Role } from '../../services/admin.service';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';

const RoleManagement: React.FC = () => {
  const [roles, setRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  // Fetch roles on component mount
  useEffect(() => {
    fetchRoles();
  }, []);

  const fetchRoles = async () => {
    try {
      setLoading(true);
      setError(null);
      const fetchedRoles = await adminService.getAllRoles();
      setRoles(fetchedRoles);
    } catch (err: any) {
      console.error('Error fetching roles:', err);
      // Enhanced error message with more details
      const errorMessage = err.response?.status === 404
        ? 'API endpoint not found. Please check the API configuration.'
        : err.response?.data?.message || err.message || 'Failed to load roles. Please try again later.';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteRole = async (roleId: string) => {
    if (!window.confirm('Are you sure you want to delete this role?')) {
      return;
    }

    try {
      setLoading(true);
      setError(null);
      setSuccessMessage(null);
      await adminService.deleteRole(roleId);
      setRoles(roles.filter(role => role.id !== roleId));
      setSuccessMessage('Role deleted successfully');
    } catch (err: any) {
      console.error('Error deleting role:', err);
      // Enhanced error message with more details
      const errorMessage = err.response?.status === 404
        ? `Role with ID ${roleId} not found or API endpoint is incorrect.`
        : err.response?.data?.message || err.message || 'Failed to delete role. Please try again later.';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  // Validation schema for role creation form
  const createRoleSchema = Yup.object().shape({
    roleName: Yup.string()
      .required('Role name is required')
      .min(2, 'Role name must be at least 2 characters')
      .max(50, 'Role name cannot exceed 50 characters'),
  });

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
      {/* Role List */}
      <div className="bg-white shadow rounded-lg p-4">
        <h2 className="text-xl font-semibold mb-4">Roles</h2>
        
        {loading && (
          <div className="flex justify-center my-4">
            <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-indigo-500"></div>
          </div>
        )}
        
        {error && (
          <div className="bg-red-100 text-red-700 p-3 rounded mb-4">
            {error}
          </div>
        )}
        
        {successMessage && (
          <div className="bg-green-100 text-green-700 p-3 rounded mb-4">
            {successMessage}
          </div>
        )}
        
        <ul className="divide-y divide-gray-200">
          {roles.map(role => (
            <li key={role.id} className="py-3 flex justify-between items-center">
              <div>
                <span className="font-medium">{role.name}</span>
              </div>
              <button
                onClick={() => handleDeleteRole(role.id)}
                className="px-2 py-1 bg-red-600 text-white text-sm rounded hover:bg-red-700"
              >
                Delete
              </button>
            </li>
          ))}
        </ul>
        
        {roles.length === 0 && !loading && (
          <p className="text-gray-500 text-center py-4">No roles found</p>
        )}
      </div>

      {/* Create Role Form */}
      <div className="bg-white shadow rounded-lg p-4">
        <h2 className="text-xl font-semibold mb-4">Create New Role</h2>
        
        <Formik
          initialValues={{ roleName: '' }}
          validationSchema={createRoleSchema}
          onSubmit={async (values, { setSubmitting, resetForm }) => {
            try {
              setError(null);
              setSuccessMessage(null);
              const newRole = await adminService.createRole(values.roleName);
              setRoles([...roles, newRole]);
              resetForm();
              setSuccessMessage('Role created successfully');
            } catch (err: any) {
              console.error('Error creating role:', err);
              // Enhanced error message with more details
              const errorMessage = err.response?.status === 404
                ? 'API endpoint not found. Please check the API configuration.'
                : err.response?.data?.message || err.message || 'Failed to create role. Please try again later.';
              setError(errorMessage);
            } finally {
              setSubmitting(false);
            }
          }}
        >
          {({ isSubmitting }) => (
            <Form className="space-y-4">
              <div>
                <label htmlFor="roleName" className="block text-sm font-medium text-gray-700">
                  Role Name
                </label>
                <Field
                  type="text"
                  name="roleName"
                  id="roleName"
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                  placeholder="Enter role name"
                />
                <ErrorMessage name="roleName" component="div" className="text-red-500 text-sm mt-1" />
              </div>

              <div className="flex justify-end">
                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="px-4 py-2 bg-indigo-600 text-white rounded hover:bg-indigo-700 disabled:bg-indigo-300"
                >
                  {isSubmitting ? 'Creating...' : 'Create Role'}
                </button>
              </div>
            </Form>
          )}
        </Formik>
      </div>
    </div>
  );
};

export default RoleManagement; 