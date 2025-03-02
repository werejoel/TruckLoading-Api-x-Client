import React, { useState, useEffect } from 'react';
import adminService, { User, AdminUpdateUserRequest } from '../../services/admin.service';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';

const UserManagement: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [isEditing, setIsEditing] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch users on component mount
  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      setError(null);
      const fetchedUsers = await adminService.getAllUsers();
      setUsers(fetchedUsers);
    } catch (err: any) {
      console.error('Error fetching users:', err);
      // Enhanced error message with more details
      const errorMessage = err.response?.status === 404
        ? 'API endpoint not found. Please check the API configuration.'
        : err.response?.data?.message || err.message || 'Failed to load users. Please try again later.';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleViewUser = async (userId: string) => {
    try {
      setLoading(true);
      setError(null);
      const user = await adminService.getUserById(userId);
      setSelectedUser(user);
      setIsEditing(false);
    } catch (err: any) {
      console.error('Error fetching user details:', err);
      // Enhanced error message with more details
      const errorMessage = err.response?.status === 404
        ? `User with ID ${userId} not found or API endpoint is incorrect.`
        : err.response?.data?.message || err.message || 'Failed to load user details. Please try again later.';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleEditUser = () => {
    setIsEditing(true);
  };

  const handleDeleteUser = async (userId: string) => {
    if (!window.confirm('Are you sure you want to delete this user?')) {
      return;
    }

    try {
      setLoading(true);
      setError(null);
      await adminService.deleteUser(userId);
      setUsers(users.filter(user => user.id !== userId));
      setSelectedUser(null);
      setIsEditing(false);
    } catch (err: any) {
      console.error('Error deleting user:', err);
      // Enhanced error message with more details
      const errorMessage = err.response?.status === 404
        ? `User with ID ${userId} not found or API endpoint is incorrect.`
        : err.response?.data?.message || err.message || 'Failed to delete user. Please try again later.';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  // Validation schema for user update form
  const updateUserSchema = Yup.object().shape({
    companyName: Yup.string().nullable(),
    companyAddress: Yup.string().nullable(),
    companyRegistrationNumber: Yup.string().nullable(),
    companyContact: Yup.string().nullable(),
  });

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
      {/* User List */}
      <div className="md:col-span-1 bg-white shadow rounded-lg p-4">
        <h2 className="text-xl font-semibold mb-4">Users</h2>
        
        {loading && !selectedUser && (
          <div className="flex justify-center my-4">
            <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-indigo-500"></div>
          </div>
        )}
        
        {error && !selectedUser && (
          <div className="bg-red-100 text-red-700 p-3 rounded mb-4">
            {error}
          </div>
        )}
        
        <ul className="divide-y divide-gray-200">
          {users.map(user => (
            <li key={user.id} className="py-3">
              <button
                onClick={() => handleViewUser(user.id)}
                className="w-full text-left hover:bg-gray-50 p-2 rounded"
              >
                <div className="font-medium">{user.firstName} {user.lastName}</div>
                <div className="text-sm text-gray-500">{user.email}</div>
              </button>
            </li>
          ))}
        </ul>
        
        {users.length === 0 && !loading && (
          <p className="text-gray-500 text-center py-4">No users found</p>
        )}
      </div>

      {/* User Details / Edit Form */}
      <div className="md:col-span-2 bg-white shadow rounded-lg p-4">
        {selectedUser ? (
          <>
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-semibold">
                {isEditing ? 'Edit User' : 'User Details'}
              </h2>
              <div className="space-x-2">
                {!isEditing ? (
                  <>
                    <button
                      onClick={handleEditUser}
                      className="px-3 py-1 bg-indigo-600 text-white rounded hover:bg-indigo-700"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => handleDeleteUser(selectedUser.id)}
                      className="px-3 py-1 bg-red-600 text-white rounded hover:bg-red-700"
                    >
                      Delete
                    </button>
                  </>
                ) : (
                  <button
                    onClick={() => setIsEditing(false)}
                    className="px-3 py-1 bg-gray-500 text-white rounded hover:bg-gray-600"
                  >
                    Cancel
                  </button>
                )}
              </div>
            </div>

            {isEditing ? (
              <Formik
                initialValues={{
                  companyName: selectedUser.companyName || '',
                  companyAddress: selectedUser.companyAddress || '',
                  companyRegistrationNumber: selectedUser.companyRegistrationNumber || '',
                  companyContact: selectedUser.companyContact || '',
                }}
                validationSchema={updateUserSchema}
                onSubmit={async (values, { setSubmitting }) => {
                  try {
                    setError(null);
                    await adminService.updateUser(selectedUser.id, values);
                    
                    // Update the user in the local state
                    const updatedUser = { ...selectedUser, ...values };
                    setSelectedUser(updatedUser);
                    setUsers(users.map(u => u.id === selectedUser.id ? updatedUser : u));
                    
                    setIsEditing(false);
                  } catch (err: any) {
                    console.error('Error updating user:', err);
                    // Enhanced error message with more details
                    const errorMessage = err.response?.status === 404
                      ? `User with ID ${selectedUser.id} not found or API endpoint is incorrect.`
                      : err.response?.data?.message || err.message || 'Failed to update user. Please try again later.';
                    setError(errorMessage);
                  } finally {
                    setSubmitting(false);
                  }
                }}
              >
                {({ isSubmitting }) => (
                  <Form className="space-y-4">
                    <div>
                      <label htmlFor="companyName" className="block text-sm font-medium text-gray-700">
                        Company Name
                      </label>
                      <Field
                        type="text"
                        name="companyName"
                        id="companyName"
                        className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                      />
                      <ErrorMessage name="companyName" component="div" className="text-red-500 text-sm mt-1" />
                    </div>

                    <div>
                      <label htmlFor="companyAddress" className="block text-sm font-medium text-gray-700">
                        Company Address
                      </label>
                      <Field
                        type="text"
                        name="companyAddress"
                        id="companyAddress"
                        className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                      />
                      <ErrorMessage name="companyAddress" component="div" className="text-red-500 text-sm mt-1" />
                    </div>

                    <div>
                      <label htmlFor="companyRegistrationNumber" className="block text-sm font-medium text-gray-700">
                        Company Registration Number
                      </label>
                      <Field
                        type="text"
                        name="companyRegistrationNumber"
                        id="companyRegistrationNumber"
                        className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                      />
                      <ErrorMessage name="companyRegistrationNumber" component="div" className="text-red-500 text-sm mt-1" />
                    </div>

                    <div>
                      <label htmlFor="companyContact" className="block text-sm font-medium text-gray-700">
                        Company Contact
                      </label>
                      <Field
                        type="text"
                        name="companyContact"
                        id="companyContact"
                        className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                      />
                      <ErrorMessage name="companyContact" component="div" className="text-red-500 text-sm mt-1" />
                    </div>

                    <div className="flex justify-end">
                      <button
                        type="submit"
                        disabled={isSubmitting}
                        className="px-4 py-2 bg-indigo-600 text-white rounded hover:bg-indigo-700 disabled:bg-indigo-300"
                      >
                        {isSubmitting ? 'Saving...' : 'Save Changes'}
                      </button>
                    </div>
                  </Form>
                )}
              </Formik>
            ) : (
              <div className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-sm font-medium text-gray-500">User ID</p>
                    <p>{selectedUser.id}</p>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-gray-500">Username</p>
                    <p>{selectedUser.userName}</p>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-gray-500">Email</p>
                    <p>{selectedUser.email}</p>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-gray-500">Name</p>
                    <p>{selectedUser.firstName} {selectedUser.lastName}</p>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-gray-500">Company Name</p>
                    <p>{selectedUser.companyName || '-'}</p>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-gray-500">Company Address</p>
                    <p>{selectedUser.companyAddress || '-'}</p>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-gray-500">Registration Number</p>
                    <p>{selectedUser.companyRegistrationNumber || '-'}</p>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-gray-500">Company Contact</p>
                    <p>{selectedUser.companyContact || '-'}</p>
                  </div>
                </div>
              </div>
            )}
          </>
        ) : (
          <div className="text-center py-8">
            <p className="text-gray-500">Select a user to view details</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default UserManagement; 