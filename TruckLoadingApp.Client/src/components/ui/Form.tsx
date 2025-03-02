import { ReactNode, forwardRef } from 'react';

interface FormGroupProps {
  children: ReactNode;
  className?: string;
}

export const FormGroup = ({ children, className = '' }: FormGroupProps) => {
  return (
    <div className={`mb-5 ${className}`}>
      {children}
    </div>
  );
};

interface FormLabelProps {
  children: ReactNode;
  htmlFor?: string;
  required?: boolean;
  className?: string;
}

export const FormLabel = ({ children, htmlFor, required = false, className = '' }: FormLabelProps) => {
  return (
    <label htmlFor={htmlFor} className={`block text-sm font-medium text-gray-700 mb-1 ${className}`}>
      {children}
      {required && <span className="ml-1 text-red-500">*</span>}
    </label>
  );
};

interface FormErrorProps {
  children: ReactNode;
  className?: string;
}

export const FormError = ({ children, className = '' }: FormErrorProps) => {
  return (
    <p className={`mt-1 text-sm text-red-600 ${className}`}>
      {children}
    </p>
  );
};

interface FormInputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  error?: string;
  className?: string;
}

export const FormInput = forwardRef<HTMLInputElement, FormInputProps>(
  ({ error, className = '', ...props }, ref) => {
    return (
      <>
        <input
          ref={ref}
          className={`appearance-none block w-full px-3 py-2 border ${
            error ? 'border-red-300' : 'border-gray-300'
          } rounded-md shadow-sm placeholder-gray-400 
          focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm ${className}`}
          {...props}
        />
        {error && <FormError>{error}</FormError>}
      </>
    );
  }
);

interface FormSelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  children: ReactNode;
  error?: string;
  className?: string;
}

export const FormSelect = forwardRef<HTMLSelectElement, FormSelectProps>(
  ({ children, error, className = '', ...props }, ref) => {
    return (
      <>
        <select
          ref={ref}
          className={`block w-full pl-3 pr-10 py-2 text-base border ${
            error ? 'border-red-300' : 'border-gray-300'
          } focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md ${className}`}
          {...props}
        >
          {children}
        </select>
        {error && <FormError>{error}</FormError>}
      </>
    );
  }
);

interface FormTextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  error?: string;
  className?: string;
}

export const FormTextarea = forwardRef<HTMLTextAreaElement, FormTextareaProps>(
  ({ error, className = '', ...props }, ref) => {
    return (
      <>
        <textarea
          ref={ref}
          className={`appearance-none block w-full px-3 py-2 border ${
            error ? 'border-red-300' : 'border-gray-300'
          } rounded-md shadow-sm placeholder-gray-400 
          focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm ${className}`}
          rows={4}
          {...props}
        />
        {error && <FormError>{error}</FormError>}
      </>
    );
  }
);

interface FormCheckboxProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'type'> {
  label: string;
  error?: string;
  className?: string;
}

export const FormCheckbox = forwardRef<HTMLInputElement, FormCheckboxProps>(
  ({ label, error, className = '', ...props }, ref) => {
    return (
      <div className="flex items-start">
        <div className="flex items-center h-5">
          <input
            ref={ref}
            type="checkbox"
            className={`focus:ring-indigo-500 h-4 w-4 text-indigo-600 border-gray-300 rounded ${className}`}
            {...props}
          />
        </div>
        <div className="ml-3 text-sm">
          <label className="font-medium text-gray-700">{label}</label>
          {error && <FormError>{error}</FormError>}
        </div>
      </div>
    );
  }
);

interface FormButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger' | 'success' | 'warning';
  size?: 'sm' | 'md' | 'lg';
  isLoading?: boolean;
}

export const FormButton = forwardRef<HTMLButtonElement, FormButtonProps>(
  ({ children, variant = 'primary', size = 'md', isLoading, className = '', disabled, ...props }, ref) => {
    const variantClasses = {
      primary: 'bg-indigo-600 hover:bg-indigo-700 text-white',
      secondary: 'bg-gray-600 hover:bg-gray-700 text-white',
      danger: 'bg-red-600 hover:bg-red-700 text-white',
      success: 'bg-green-600 hover:bg-green-700 text-white',
      warning: 'bg-yellow-500 hover:bg-yellow-600 text-white'
    };
    
    const sizeClasses = {
      sm: 'px-3 py-1.5 text-sm',
      md: 'px-4 py-2',
      lg: 'px-6 py-3 text-lg'
    };
    
    return (
      <button
        ref={ref}
        disabled={disabled || isLoading}
        className={`inline-flex justify-center items-center border border-transparent 
          rounded-md font-medium shadow-sm focus:outline-none focus:ring-2 
          focus:ring-offset-2 focus:ring-indigo-500 ${sizeClasses[size]} 
          ${variantClasses[variant]} ${disabled || isLoading ? 'opacity-50 cursor-not-allowed' : ''} ${className}`}
        {...props}
      >
        {isLoading && (
          <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
        )}
        {children}
      </button>
    );
  }
);

FormInput.displayName = 'FormInput';
FormSelect.displayName = 'FormSelect';
FormTextarea.displayName = 'FormTextarea';
FormCheckbox.displayName = 'FormCheckbox';
FormButton.displayName = 'FormButton';
