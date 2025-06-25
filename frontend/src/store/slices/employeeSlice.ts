import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { employeeAPI } from '../../services/api';

export interface Employee {
  employeeId: number;
  employeeCode: string;
  fullName: string;
  email: string;
  title?: string;
  level: string;
  isActive: boolean;
  hireDate?: string;
  terminationDate?: string;
  homeOffice?: {
    officeId: number;
    officeName: string;
    region: string;
  };
  affiliations: EmployeeAffiliation[];
  roles: EmployeeRole[];
}

export interface EmployeeAffiliation {
  affiliationId: number;
  roleType: string;
  practice: string;
  locationScope?: string;
  effectiveDate: string;
  isActive: boolean;
}

export interface EmployeeRole {
  employeeRoleId: number;
  roleType: string;
  primaryPractice: string;
  secondaryPractice?: string;
  locationScope?: string;
  isActive: boolean;
}

export interface EmployeeState {
  employees: Employee[];
  selectedEmployee: Employee | null;
  isLoading: boolean;
  isLoadingDetail: boolean;
  error: string | null;
  totalCount: number;
  filters: {
    search: string;
    office: string;
    level: string;
    status: string;
  };
}

const initialState: EmployeeState = {
  employees: [],
  selectedEmployee: null,
  isLoading: false,
  isLoadingDetail: false,
  error: null,
  totalCount: 0,
  filters: {
    search: '',
    office: '',
    level: '',
    status: '',
  },
};

// Async thunks
export const fetchEmployees = createAsyncThunk(
  'employees/fetchEmployees',
  async (_, { rejectWithValue }) => {
    try {
      const response = await employeeAPI.getAll();
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch employees');
    }
  }
);

export const fetchEmployeeDetail = createAsyncThunk(
  'employees/fetchEmployeeDetail',
  async (employeeId: number, { rejectWithValue }) => {
    try {
      const response = await employeeAPI.getById(employeeId);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch employee details');
    }
  }
);

export const createEmployee = createAsyncThunk(
  'employees/createEmployee',
  async (employeeData: Partial<Employee>, { rejectWithValue }) => {
    try {
      const response = await employeeAPI.create(employeeData);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to create employee');
    }
  }
);

export const updateEmployee = createAsyncThunk(
  'employees/updateEmployee',
  async ({ id, data }: { id: number; data: Partial<Employee> }, { rejectWithValue }) => {
    try {
      const response = await employeeAPI.update(id, data);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to update employee');
    }
  }
);

export const deleteEmployee = createAsyncThunk(
  'employees/deleteEmployee',
  async (employeeId: number, { rejectWithValue }) => {
    try {
      await employeeAPI.delete(employeeId);
      return employeeId;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to delete employee');
    }
  }
);

export const searchEmployees = createAsyncThunk(
  'employees/searchEmployees',
  async (query: string, { rejectWithValue }) => {
    try {
      const response = await employeeAPI.search(query);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to search employees');
    }
  }
);

const employeeSlice = createSlice({
  name: 'employees',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    clearSelectedEmployee: (state) => {
      state.selectedEmployee = null;
    },
    setFilters: (state, action: PayloadAction<Partial<EmployeeState['filters']>>) => {
      state.filters = { ...state.filters, ...action.payload };
    },
    clearFilters: (state) => {
      state.filters = initialState.filters;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch employees
      .addCase(fetchEmployees.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(fetchEmployees.fulfilled, (state, action) => {
        state.isLoading = false;
        state.employees = action.payload;
        state.totalCount = action.payload.length;
      })
      .addCase(fetchEmployees.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      })
      
      // Fetch employee detail
      .addCase(fetchEmployeeDetail.pending, (state) => {
        state.isLoadingDetail = true;
        state.error = null;
      })
      .addCase(fetchEmployeeDetail.fulfilled, (state, action) => {
        state.isLoadingDetail = false;
        state.selectedEmployee = action.payload;
      })
      .addCase(fetchEmployeeDetail.rejected, (state, action) => {
        state.isLoadingDetail = false;
        state.error = action.payload as string;
      })
      
      // Create employee
      .addCase(createEmployee.fulfilled, (state, action) => {
        state.employees.push(action.payload);
        state.totalCount += 1;
      })
      
      // Update employee
      .addCase(updateEmployee.fulfilled, (state, action) => {
        const index = state.employees.findIndex(emp => emp.employeeId === action.payload.employeeId);
        if (index !== -1) {
          state.employees[index] = action.payload;
        }
        if (state.selectedEmployee?.employeeId === action.payload.employeeId) {
          state.selectedEmployee = action.payload;
        }
      })
      
      // Delete employee
      .addCase(deleteEmployee.fulfilled, (state, action) => {
        state.employees = state.employees.filter(emp => emp.employeeId !== action.payload);
        state.totalCount -= 1;
        if (state.selectedEmployee?.employeeId === action.payload) {
          state.selectedEmployee = null;
        }
      })
      
      // Search employees
      .addCase(searchEmployees.fulfilled, (state, action) => {
        state.employees = action.payload;
        state.totalCount = action.payload.length;
      });
  },
});

export const { clearError, clearSelectedEmployee, setFilters, clearFilters } = employeeSlice.actions;

// Selectors
export const selectEmployees = (state: { employees: EmployeeState }) => state.employees.employees;
export const selectSelectedEmployee = (state: { employees: EmployeeState }) => state.employees.selectedEmployee;
export const selectIsLoading = (state: { employees: EmployeeState }) => state.employees.isLoading;
export const selectIsLoadingDetail = (state: { employees: EmployeeState }) => state.employees.isLoadingDetail;
export const selectError = (state: { employees: EmployeeState }) => state.employees.error;
export const selectTotalCount = (state: { employees: EmployeeState }) => state.employees.totalCount;
export const selectFilters = (state: { employees: EmployeeState }) => state.employees.filters;

export default employeeSlice.reducer; 