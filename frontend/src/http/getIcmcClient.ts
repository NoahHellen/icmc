import localConfig from '@config/config';
import axios from 'axios';
import authInterceptor from './authInterceptor';

const getIcmcApiClient = async () => {
  const icmcClient = axios.create({
    baseURL: localConfig.EndpointConfig.Api,
  });
  await authInterceptor(icmcClient);
  return icmcClient;
};

export default getIcmcApiClient;
