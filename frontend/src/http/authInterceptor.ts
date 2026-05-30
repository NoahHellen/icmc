import { getToken } from '@utils/authStorage';
import type { AxiosHeaders, AxiosInstance } from 'axios';

const authInterceptor = async (axiosInstance: AxiosInstance) => {
  axiosInstance.interceptors.request.use(
    async (config) => {
      const jwtToken = await getToken();
      if (jwtToken) {
        const bearer = `Bearer ${jwtToken}`;
        (config.headers as AxiosHeaders).set('Authorization', bearer);
      }
      return config;
    },
    (error) => {
      console.error(error);
      return Promise.reject(error);
    }
  );
};

export default authInterceptor;
