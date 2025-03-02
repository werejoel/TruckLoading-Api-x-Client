import { ReactNode } from 'react';

interface Column {
  header: string;
  accessor: string;
  render?: (value: any, item: any) => ReactNode;
}

interface TableProps<T> {
  columns: Column[];
  data: T[];
  keyField: keyof T | ((item: T) => string);
  className?: string;
  striped?: boolean;
  hoverable?: boolean;
  isLoading?: boolean;
  emptyMessage?: string;
}

export const Table = <T extends Record<string, any>>({
  columns,
  data,
  keyField,
  className = '',
  striped = true,
  hoverable = true,
  isLoading = false,
  emptyMessage = 'No data available'
}: TableProps<T>) => {
  const baseClasses = 'min-w-full divide-y divide-gray-200';
  
  const getKey = (item: T): string => {
    if (typeof keyField === 'function') {
      return keyField(item);
    }
    return String(item[keyField]);
  };

  if (isLoading) {
    return (
      <div className="w-full py-12 flex items-center justify-center">
        <svg className="animate-spin h-8 w-8 text-indigo-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
      </div>
    );
  }

  if (data.length === 0) {
    return (
      <div className="w-full py-12 flex items-center justify-center text-gray-500">
        {emptyMessage}
      </div>
    );
  }

  return (
    <div className="overflow-x-auto">
      <table className={`${baseClasses} ${className}`}>
        <thead className="bg-gray-50">
          <tr>
            {columns.map((column, index) => (
              <th
                key={index}
                scope="col"
                className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
              >
                {column.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="bg-white divide-y divide-gray-200">
          {data.map((item, rowIndex) => (
            <tr 
              key={getKey(item)}
              className={`
                ${hoverable ? 'hover:bg-gray-50' : ''}
                ${striped && rowIndex % 2 === 1 ? 'bg-gray-50' : ''}
              `}
            >
              {columns.map((column, colIndex) => (
                <td 
                  key={colIndex} 
                  className="px-6 py-4 whitespace-nowrap text-sm text-gray-500"
                >
                  {column.render 
                    ? column.render(item[column.accessor], item) 
                    : item[column.accessor]
                  }
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
