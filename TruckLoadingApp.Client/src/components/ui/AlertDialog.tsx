import { ReactNode } from 'react';
import { Modal } from './Modal';
import { AnimatedButton } from './AnimatedButton';

interface AlertDialogProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  confirmLabel?: string;
  cancelLabel?: string;
  onConfirm: () => void;
  variant?: 'danger' | 'warning' | 'info';
  isLoading?: boolean;
}

export const AlertDialog = ({
  isOpen,
  onClose,
  title,
  children,
  confirmLabel = 'Confirm',
  cancelLabel = 'Cancel',
  onConfirm,
  variant = 'danger',
  isLoading = false
}: AlertDialogProps) => {
  const getVariantButton = () => {
    switch (variant) {
      case 'danger':
        return 'danger';
      case 'warning':
        return 'secondary';
      case 'info':
        return 'primary';
      default:
        return 'danger';
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} maxWidth="sm">
      <div>
        <div className="text-center sm:text-left">
          <h3 className="text-lg leading-6 font-medium text-gray-900">{title}</h3>
          <div className="mt-2">
            {children}
          </div>
        </div>
      </div>
      <div className="mt-5 sm:mt-4 sm:flex sm:flex-row-reverse">
        <AnimatedButton
          type="button"
          variant={getVariantButton()}
          className="w-full sm:ml-3 sm:w-auto"
          onClick={onConfirm}
          isLoading={isLoading}
        >
          {confirmLabel}
        </AnimatedButton>
        <AnimatedButton
          type="button"
          variant="outline"
          className="mt-3 sm:mt-0 w-full sm:w-auto"
          onClick={onClose}
          disabled={isLoading}
        >
          {cancelLabel}
        </AnimatedButton>
      </div>
    </Modal>
  );
};
