import { FaHome, FaTruck, FaUsers, FaChartLine } from 'react-icons/fa';

interface NavigationItem {
  path: string;
  label: string;
  icon?: string;
}

export const navigationConfig = {
  company: [
    { path: '/company/dashboard', label: 'Dashboard', icon: 'FaHome' },
    { path: '/company/drivers', label: 'Drivers', icon: 'FaUsers' },
    { path: '/company/trucks', label: 'Trucks', icon: 'FaTruck' },
    { path: '/company/analytics', label: 'Analytics', icon: 'FaChartLine' }
  ] as NavigationItem[]
}; 