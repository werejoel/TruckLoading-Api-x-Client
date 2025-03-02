import React from 'react';

type BadgeVariant = 'gray' | 'red' | 'yellow' | 'green' | 'blue' | 'indigo' | 'purple' | 'pink';

interface BadgeProps {
  children: React.ReactNode;
  variant?: BadgeVariant;
  size?: 'sm' | 'md' | 'lg';
  rounded?: 'sm' | 'md' | 'lg' | 'full';
  className?: string;
}

export const Badge: React.FC<BadgeProps> = ({
  children,
  variant = 'gray',
  size = 'md',
  rounded = 'full',
  className = '',
  ...props
}) => {
  const variantClasses = {
    gray: 'bg-gray-100 text-gray-800',
    red: 'bg-red-100 text-red-800',
    yellow: 'bg-yellow-100 text-yellow-800',
    green: 'bg-green-100 text-green-800',
    blue: 'bg-blue-100 text-blue-800',
    indigo: 'bg-indigo-100 text-indigo-800',
    purple: 'bg-purple-100 text-purple-800',
    pink: 'bg-pink-100 text-pink-800',
  };

  const sizeClasses = {
    sm: 'px-2 py-0.5 text-xs',
    md: 'px-2.5 py-0.5 text-sm',
    lg: 'px-3 py-0.5 text-base',
  };

  const roundedClasses = {
    sm: 'rounded',
    md: 'rounded-md',
    lg: 'rounded-lg',
    full: 'rounded-full',
  };

  return (
    <span
      className={`inline-flex items-center font-medium ${sizeClasses[size]} ${roundedClasses[rounded]} ${variantClasses[variant]} ${className}`}
      {...props}
    >
      {children}
    </span>
  );
};

export const StatusBadge: React.FC<{ status: string; className?: string }> = ({ status, className = '' }) => {
  let variant: BadgeVariant = 'gray';
  
  switch (status.toLowerCase()) {
    case 'active':
    case 'completed':
    case 'approved':
    case 'success':
      variant = 'green';
      break;
    case 'pending':
    case 'in progress':
    case 'processing':
      variant = 'blue';
      break;
    case 'warning':
    case 'on hold':
      variant = 'yellow';
      break;
    case 'error':
    case 'rejected':
    case 'cancelled':
    case 'failed':
      variant = 'red';
      break;
    default:
      variant = 'gray';
  }

  return (
    <Badge variant={variant} className={className}>
      {status}
    </Badge>
  );
};
