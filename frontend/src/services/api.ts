import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'https://726729e2-0245-4990-b707-7eddbbbf3c34-00-220kpho6o8ybn.sisko.replit.dev:5000/api';

// Create axios instance with default config
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Handle unauthorized access
      localStorage.removeItem('accessToken');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Employee API
export const employeeAPI = {
  getAll: () => api.get('/Employees'),
  getById: (id: number) => api.get(`/Employees/${id}`),
  create: (data: any) => api.post('/Employees', data),
  update: (id: number, data: any) => api.put(`/Employees/${id}`, data),
  delete: (id: number) => api.delete(`/Employees/${id}`),
  search: (query: string) => api.get(`/Employees/search?q=${query}`),
};

// Affiliation API
export const affiliationAPI = {
  getAll: () => api.get('/Affiliations'),
  getById: (id: number) => api.get(`/Affiliations/${id}`),
  create: (data: any) => api.post('/Affiliations', data),
  update: (id: number, data: any) => api.put(`/Affiliations/${id}`, data),
  delete: (id: number) => api.delete(`/Affiliations/${id}`),
  getByEmployee: (employeeId: number) => api.get(`/Employees/${employeeId}/Affiliations`),
};

// Office API
export const officeAPI = {
  getAll: () => api.get('/Offices'),
  getById: (id: number) => api.get(`/Offices/${id}`),
};

// Taxonomy API
export const taxonomyAPI = {
  getPractices: () => api.get('/Taxonomy/Practices'),
  getRoleTypes: () => api.get('/Taxonomy/RoleTypes'),
  getLevels: () => api.get('/Taxonomy/Levels'),
};

// Reports API
export const reportsAPI = {
  getEmployeeReport: (filters?: any) => api.get('/Reports/Employees', { params: filters }),
  getAffiliationReport: (filters?: any) => api.get('/Reports/Affiliations', { params: filters }),
  getDashboardStats: () => api.get('/Reports/Dashboard'),
};

// Auth API
export const authAPI = {
  login: (credentials: any) => api.post('/Auth/Login', credentials),
  logout: () => api.post('/Auth/Logout'),
  getCurrentUser: () => api.get('/Auth/Me'),
  refreshToken: () => api.post('/Auth/Refresh'),
};

export default api; 