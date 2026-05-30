import queryClient from '@api/queryClient';
import { type QueryKey, useMutation, useQuery } from '@tanstack/react-query';
import { addLog, searchLogbook } from './logbookService';
import type {
  AddLogRequest,
  LogbookDto,
  SearchLogbookRequest,
} from './logbookTypes';

export function getLogbookKey(request?: SearchLogbookRequest): QueryKey {
  return ['logbook', request];
}

export function useSearchLogbook(request?: SearchLogbookRequest) {
  const query = useQuery<LogbookDto[]>({
    queryKey: getLogbookKey(request),
    queryFn: () => searchLogbook(request),
    placeholderData: (previousData) => previousData,
  });
  return query;
}

export function useAddLog() {
  const mutation = useMutation({
    mutationFn: (request: AddLogRequest) => addLog(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['logbook'] });
    },
  });
  return mutation;
}
