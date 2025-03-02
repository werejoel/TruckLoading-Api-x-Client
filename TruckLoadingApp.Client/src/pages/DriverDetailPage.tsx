import React from 'react';
import DriverDetail from '../components/driver/DriverDetail';
import Layout from '../components/layout/Layout';

const DriverDetailPage: React.FC = () => {
  return (
    <Layout>
      <div className="container mx-auto px-4 py-8">
        <DriverDetail />
      </div>
    </Layout>
  );
};

export default DriverDetailPage; 