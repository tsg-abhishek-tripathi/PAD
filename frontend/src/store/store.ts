import { configureStore } from '@reduxjs/toolkit';
import employeeReducer from './slices/employeeSlice';
import affiliationReducer from './slices/affiliationSlice';
import authReducer from './slices/authSlice';

export const store = configureStore({
  reducer: {
    employees: employeeReducer,
    affiliations: affiliationReducer,
    auth: authReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: ['persist/PERSIST'],
      },
    }),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch; 