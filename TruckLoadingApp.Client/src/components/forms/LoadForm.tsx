import React from 'react';
import LoadFormWizard from './LoadFormWizard';
import { Load } from '../../services/shipper.service';

interface LoadFormProps {
  initialData?: Load;
  onSuccess?: (load: Load) => void;
  onCancel?: () => void;
  isEdit?: boolean;
}

const LoadForm: React.FC<LoadFormProps> = (props) => {
  return <LoadFormWizard {...props} />;
};

export default LoadForm;