import React from 'react';

interface PageTemplateProps {
  title: string;
  children: React.ReactNode;
  actions?: React.ReactNode;
}

const PageTemplate: React.FC<PageTemplateProps> = ({ title, children, actions }) => {
  return (
    <div className="py-6">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 md:px-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-semibold text-gray-900">{title}</h1>
          {actions && <div className="flex space-x-3">{actions}</div>}
        </div>
        <div className="bg-white shadow rounded-lg p-6">
          {children}
        </div>
      </div>
    </div>
  );
};

export default PageTemplate; 