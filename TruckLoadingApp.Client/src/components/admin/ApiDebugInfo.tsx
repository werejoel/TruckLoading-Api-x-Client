import React, { useState } from 'react';
import api from '../../services/api';
import axios from 'axios';

const ApiDebugInfo: React.FC = () => {
  const [apiResponse, setApiResponse] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [customEndpoint, setCustomEndpoint] = useState('');

  // Get the base URL from the API service
  const baseURL = (api.defaults.baseURL || '').toString();

  const testApiConnection = async () => {
    try {
      setLoading(true);
      setError(null);
      setApiResponse(null);
      
      // Make a simple request to check if the API is reachable
      const response = await axios.get(`${baseURL}/health`, { timeout: 5000 });
      setApiResponse(JSON.stringify(response.data, null, 2));
    } catch (err: any) {
      console.error('API connection test failed:', err);
      setError(`API connection test failed: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  const testCustomEndpoint = async () => {
    if (!customEndpoint) {
      setError('Please enter an endpoint to test');
      return;
    }

    try {
      setLoading(true);
      setError(null);
      setApiResponse(null);
      
      // Make a request to the custom endpoint
      const fullUrl = customEndpoint.startsWith('http') 
        ? customEndpoint 
        : `${baseURL}/${customEndpoint.replace(/^\//, '')}`;
      
      const response = await axios.get(fullUrl, { timeout: 5000 });
      setApiResponse(JSON.stringify(response.data, null, 2));
    } catch (err: any) {
      console.error('Custom endpoint test failed:', err);
      setError(`Custom endpoint test failed: ${err.message}`);
      
      // Show response details if available
      if (err.response) {
        setApiResponse(JSON.stringify({
          status: err.response.status,
          statusText: err.response.statusText,
          data: err.response.data
        }, null, 2));
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-white shadow rounded-lg p-4">
      <h2 className="text-xl font-semibold mb-4">API Debug Information</h2>
      
      <div className="mb-4">
        <p className="text-sm font-medium text-gray-500">Base URL</p>
        <p className="font-mono bg-gray-100 p-2 rounded">{baseURL}</p>
      </div>
      
      <div className="mb-4">
        <button
          onClick={testApiConnection}
          disabled={loading}
          className="px-4 py-2 bg-indigo-600 text-white rounded hover:bg-indigo-700 disabled:bg-indigo-300 mr-2"
        >
          {loading ? 'Testing...' : 'Test API Connection'}
        </button>
      </div>
      
      <div className="mb-4">
        <label htmlFor="customEndpoint" className="block text-sm font-medium text-gray-700 mb-1">
          Test Custom Endpoint
        </label>
        <div className="flex">
          <input
            type="text"
            id="customEndpoint"
            value={customEndpoint}
            onChange={(e) => setCustomEndpoint(e.target.value)}
            placeholder="e.g., v1/admin/users"
            className="flex-1 rounded-l-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
          />
          <button
            onClick={testCustomEndpoint}
            disabled={loading}
            className="px-4 py-2 bg-indigo-600 text-white rounded-r-md hover:bg-indigo-700 disabled:bg-indigo-300"
          >
            Test
          </button>
        </div>
      </div>
      
      {error && (
        <div className="bg-red-100 text-red-700 p-3 rounded mb-4">
          {error}
        </div>
      )}
      
      {apiResponse && (
        <div className="mb-4">
          <p className="text-sm font-medium text-gray-500 mb-1">API Response</p>
          <pre className="font-mono bg-gray-100 p-2 rounded overflow-auto max-h-60">
            {apiResponse}
          </pre>
        </div>
      )}
      
      <div className="mt-4 text-sm text-gray-500">
        <p>Common issues:</p>
        <ul className="list-disc pl-5 mt-1">
          <li>Incorrect API base URL</li>
          <li>CORS issues (check browser console)</li>
          <li>API endpoint not implemented</li>
          <li>Authentication issues</li>
        </ul>
      </div>
    </div>
  );
};

export default ApiDebugInfo; 