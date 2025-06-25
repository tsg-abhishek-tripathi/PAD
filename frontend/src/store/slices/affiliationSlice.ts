import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { affiliationAPI } from '../../services/api';

export interface Affiliation {
  affiliationId: number;
  employeeId: number;
  employeeName: string;
  roleType: string;
  practice: string;
  locationScope?: string;
  effectiveDate: string;
  expirationDate?: string;
  isActive: boolean;
  createdDate: string;
  lastModifiedDate: string;
}

export interface AffiliationState {
  affiliations: Affiliation[];
  selectedAffiliation: Affiliation | null;
  isLoading: boolean;
  isLoadingDetail: boolean;
  error: string | null;
  totalCount: number;
  filters: {
    search: string;
    practice: string;
    roleType: string;
    status: string;
  };
}

const initialState: AffiliationState = {
  affiliations: [],
  selectedAffiliation: null,
  isLoading: false,
  isLoadingDetail: false,
  error: null,
  totalCount: 0,
  filters: {
    search: '',
    practice: '',
    roleType: '',
    status: '',
  },
};

// Async thunks
export const fetchAffiliations = createAsyncThunk(
  'affiliations/fetchAffiliations',
  async (_, { rejectWithValue }) => {
    try {
      const response = await affiliationAPI.getAll();
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch affiliations');
    }
  }
);

export const fetchAffiliationDetail = createAsyncThunk(
  'affiliations/fetchAffiliationDetail',
  async (affiliationId: number, { rejectWithValue }) => {
    try {
      const response = await affiliationAPI.getById(affiliationId);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch affiliation details');
    }
  }
);

export const createAffiliation = createAsyncThunk(
  'affiliations/createAffiliation',
  async (affiliationData: Partial<Affiliation>, { rejectWithValue }) => {
    try {
      const response = await affiliationAPI.create(affiliationData);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to create affiliation');
    }
  }
);

export const updateAffiliation = createAsyncThunk(
  'affiliations/updateAffiliation',
  async ({ id, data }: { id: number; data: Partial<Affiliation> }, { rejectWithValue }) => {
    try {
      const response = await affiliationAPI.update(id, data);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to update affiliation');
    }
  }
);

export const deleteAffiliation = createAsyncThunk(
  'affiliations/deleteAffiliation',
  async (affiliationId: number, { rejectWithValue }) => {
    try {
      await affiliationAPI.delete(affiliationId);
      return affiliationId;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to delete affiliation');
    }
  }
);

export const fetchEmployeeAffiliations = createAsyncThunk(
  'affiliations/fetchEmployeeAffiliations',
  async (employeeId: number, { rejectWithValue }) => {
    try {
      const response = await affiliationAPI.getByEmployee(employeeId);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch employee affiliations');
    }
  }
);

const affiliationSlice = createSlice({
  name: 'affiliations',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    clearSelectedAffiliation: (state) => {
      state.selectedAffiliation = null;
    },
    setFilters: (state, action: PayloadAction<Partial<AffiliationState['filters']>>) => {
      state.filters = { ...state.filters, ...action.payload };
    },
    clearFilters: (state) => {
      state.filters = initialState.filters;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch affiliations
      .addCase(fetchAffiliations.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(fetchAffiliations.fulfilled, (state, action) => {
        state.isLoading = false;
        state.affiliations = action.payload;
        state.totalCount = action.payload.length;
      })
      .addCase(fetchAffiliations.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      })
      
      // Fetch affiliation detail
      .addCase(fetchAffiliationDetail.pending, (state) => {
        state.isLoadingDetail = true;
        state.error = null;
      })
      .addCase(fetchAffiliationDetail.fulfilled, (state, action) => {
        state.isLoadingDetail = false;
        state.selectedAffiliation = action.payload;
      })
      .addCase(fetchAffiliationDetail.rejected, (state, action) => {
        state.isLoadingDetail = false;
        state.error = action.payload as string;
      })
      
      // Create affiliation
      .addCase(createAffiliation.fulfilled, (state, action) => {
        state.affiliations.push(action.payload);
        state.totalCount += 1;
      })
      
      // Update affiliation
      .addCase(updateAffiliation.fulfilled, (state, action) => {
        const index = state.affiliations.findIndex(aff => aff.affiliationId === action.payload.affiliationId);
        if (index !== -1) {
          state.affiliations[index] = action.payload;
        }
        if (state.selectedAffiliation?.affiliationId === action.payload.affiliationId) {
          state.selectedAffiliation = action.payload;
        }
      })
      
      // Delete affiliation
      .addCase(deleteAffiliation.fulfilled, (state, action) => {
        state.affiliations = state.affiliations.filter(aff => aff.affiliationId !== action.payload);
        state.totalCount -= 1;
        if (state.selectedAffiliation?.affiliationId === action.payload) {
          state.selectedAffiliation = null;
        }
      })
      
      // Fetch employee affiliations
      .addCase(fetchEmployeeAffiliations.fulfilled, (state, action) => {
        state.affiliations = action.payload;
        state.totalCount = action.payload.length;
      });
  },
});

export const { clearError, clearSelectedAffiliation, setFilters, clearFilters } = affiliationSlice.actions;

// Selectors
export const selectAffiliations = (state: { affiliations: AffiliationState }) => state.affiliations.affiliations;
export const selectSelectedAffiliation = (state: { affiliations: AffiliationState }) => state.affiliations.selectedAffiliation;
export const selectIsLoading = (state: { affiliations: AffiliationState }) => state.affiliations.isLoading;
export const selectIsLoadingDetail = (state: { affiliations: AffiliationState }) => state.affiliations.isLoadingDetail;
export const selectError = (state: { affiliations: AffiliationState }) => state.affiliations.error;
export const selectTotalCount = (state: { affiliations: AffiliationState }) => state.affiliations.totalCount;
export const selectFilters = (state: { affiliations: AffiliationState }) => state.affiliations.filters;

export default affiliationSlice.reducer; 