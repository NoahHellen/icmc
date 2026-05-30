import getIcmcApiClient from '@http/getIcmcClient';
import type {
  AddLogRequest,
  LogbookDto,
  SearchLogbookRequest,
} from './logbookTypes';

export const searchLogbook = async (
  request?: SearchLogbookRequest
): Promise<LogbookDto[]> => {
  const icmcClient = await getIcmcApiClient();
  const result = await icmcClient.get('/logbook', { params: request });
  return result.data;
};

export const addLog = async (request: AddLogRequest): Promise<void> => {
  const icmcClient = await getIcmcApiClient();
  const result = await icmcClient.post('/logbook', request);
  return result.data;
};
