import { useSearchLogbook } from '@api/logbook/logbookApi';
import type { LogbookDto } from '@api/logbook/logbookTypes';
import DateTimePicker, {
  type DateTimePickerEvent,
} from '@react-native-community/datetimepicker';
import { useFocusEffect } from '@react-navigation/native';
import { colours, spacing } from '@styles/variables';
import BackgroundComponent from '@ui/BackgroundComponent';
import { Card } from '@ui/Card';
import { Body, Heading, Label, Subheading } from '@ui/Typography';
import {
  Calendar,
  ChevronLeft,
  ChevronRight,
  Search,
  User,
} from 'lucide-react-native';
import { useCallback, useEffect, useMemo, useState } from 'react';
import {
  ActivityIndicator,
  FlatList,
  Platform,
  RefreshControl,
  StyleSheet,
  TextInput,
  TouchableOpacity,
  View,
} from 'react-native';

const getLocalDateString = (date: Date) => {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
};

const formatDate = (dateString?: string) => {
  if (!dateString) return 'Present';
  return new Date(dateString).toLocaleDateString(undefined, {
    day: 'numeric',
    month: 'short',
    year: '2-digit',
  });
};

const Logbook = () => {
  const [search, setSearch] = useState('');
  const [debouncedSearch, setDebouncedSearch] = useState('');

  const [selectedDate, setSelectedDate] = useState(
    getLocalDateString(new Date())
  );
  const [viewDate, setViewDate] = useState(new Date());
  const [showDatePicker, setShowDatePicker] = useState(false);

  useFocusEffect(
    useCallback(() => {
      const today = new Date();
      setSelectedDate(getLocalDateString(today));
      setViewDate(today);
    }, [])
  );

  const handleDateChange = useCallback(
    (event: DateTimePickerEvent, date?: Date) => {
      if (Platform.OS === 'android') {
        setShowDatePicker(false);
      }
      if (date) {
        setViewDate(date);
        setSelectedDate(getLocalDateString(date));
      }
    },
    []
  );

  // Debounce search term
  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedSearch(search);
    }, 400);

    return () => {
      clearTimeout(handler);
    };
  }, [search]);

  const {
    data: allLogEntries,
    isLoading,
    refetch,
    isFetching,
  } = useSearchLogbook({ search: debouncedSearch });

  const [isManualRefreshing, setIsManualRefreshing] = useState(false);

  const onRefresh = async () => {
    setIsManualRefreshing(true);
    await refetch();
    setIsManualRefreshing(false);
  };

  const countsByDate = useMemo(() => {
    if (!allLogEntries) return {};
    const counts: Record<string, number> = {};
    for (const entry of allLogEntries) {
      if (entry.returnedDate) {
        const dateKey = entry.returnedDate.split('T')[0];
        counts[dateKey] = (counts[dateKey] || 0) + 1;
      }
    }
    return counts;
  }, [allLogEntries]);

  const filteredEntries = useMemo(() => {
    if (!allLogEntries) return [];
    return allLogEntries.filter((entry) => {
      // Date filter
      const entryDate = entry.returnedDate?.split('T')[0];
      return entryDate === selectedDate;
    });
  }, [allLogEntries, selectedDate]);

  const daysInMonth = useMemo(() => {
    const year = viewDate.getFullYear();
    const month = viewDate.getMonth();
    const date = new Date(year, month, 1);
    const days = [];
    while (date.getMonth() === month) {
      days.push(new Date(date));
      date.setDate(date.getDate() + 1);
    }
    return days;
  }, [viewDate]);

  const changeMonth = useCallback((offset: number) => {
    setViewDate((prev) => {
      const newDate = new Date(prev);
      newDate.setMonth(newDate.getMonth() + offset);
      return newDate;
    });
  }, []);

  const renderDay = useCallback(
    ({ item }: { item: Date }) => {
      const dateKey = getLocalDateString(item);
      const isSelected = dateKey === selectedDate;
      const count = countsByDate[dateKey] || 0;
      const isToday = getLocalDateString(new Date()) === dateKey;

      return (
        <TouchableOpacity
          style={[
            styles.dayCard,
            isSelected && styles.dayCardSelected,
            isToday && !isSelected && styles.dayCardToday,
          ]}
          onPress={() => setSelectedDate(dateKey)}
        >
          <Label style={[styles.dayName, isSelected && styles.textSelected]}>
            {item
              .toLocaleDateString(undefined, { weekday: 'short' })
              .toUpperCase()}
          </Label>
          <Subheading
            style={[styles.dayNumber, isSelected && styles.textSelected]}
          >
            {item.getDate()}
          </Subheading>
          {count > 0 && (
            <View
              style={[styles.indicator, isSelected && styles.indicatorSelected]}
            >
              <Label style={styles.indicatorText}>{count}</Label>
            </View>
          )}
        </TouchableOpacity>
      );
    },
    [selectedDate, countsByDate]
  );

  const renderLogEntry = useCallback(
    ({ item }: { item: LogbookDto }) => (
      <Card style={styles.logCard}>
        <View style={styles.logHeader}>
          <View style={styles.gearInfo}>
            <Subheading style={styles.brand}>{item.gearItemBrand}</Subheading>
            <Body style={styles.model}>
              {item.gearItemModel || 'Unknown Model'}
            </Body>
          </View>
          <View style={styles.tagBadge}>
            <Label style={styles.tagText}>#{item.gearItemToughTag}</Label>
          </View>
        </View>

        <View style={styles.divider} />

        <View style={styles.logDetails}>
          <View style={styles.detailItem}>
            <Label style={styles.detailLabel}>DATES</Label>
            <View style={styles.detailRow}>
              <Calendar size={14} color={colours.pink} />
              <Body style={styles.detailValue}>
                {formatDate(item.lentDate)} - {formatDate(item.returnedDate)}
              </Body>
            </View>
          </View>
        </View>

        <View style={[styles.logDetails, { marginTop: spacing.small }]}>
          <View style={styles.detailItem}>
            <Label style={styles.detailLabel}>LENT TO</Label>
            <View style={styles.detailRow}>
              <User size={14} color={colours.blue} />
              <Body style={styles.detailValue} numberOfLines={1}>
                {item.lentToUserFullName || 'Unknown'}
              </Body>
            </View>
          </View>

          <View style={styles.detailItem}>
            <Label style={styles.detailLabel}>LENT BY</Label>
            <View style={styles.detailRow}>
              <User size={14} color={colours.purple} />
              <Body style={styles.detailValue} numberOfLines={1}>
                {item.lentByUserFullName || 'Unknown'}
              </Body>
            </View>
          </View>
        </View>

        {item.notes && (
          <View style={styles.notesRow}>
            <Label style={styles.notesLabel}>NOTES</Label>
            <Body style={styles.notesText}>{item.notes}</Body>
          </View>
        )}
      </Card>
    ),
    []
  );

  return (
    <BackgroundComponent>
      <View style={styles.container}>
        <View style={styles.header}>
          <View>
            <Heading>Logbook</Heading>
            <Body color={colours.textMuted}>
              {filteredEntries.length} items on this date
            </Body>
          </View>
          {(isFetching || isLoading) && (
            <ActivityIndicator color={colours.blue} size="small" />
          )}
        </View>

        <View style={styles.calendarContainer}>
          <View style={styles.monthHeader}>
            <TouchableOpacity
              onPress={() => changeMonth(-1)}
              style={styles.navButton}
            >
              <ChevronLeft size={20} color={colours.blue} />
            </TouchableOpacity>
            <TouchableOpacity
              onPress={() => setShowDatePicker(true)}
              style={styles.monthTitleButton}
            >
              <Subheading style={styles.monthTitle}>
                {viewDate.toLocaleDateString(undefined, {
                  month: 'long',
                  year: 'numeric',
                })}
              </Subheading>
            </TouchableOpacity>
            <TouchableOpacity
              onPress={() => changeMonth(1)}
              style={styles.navButton}
            >
              <ChevronRight size={20} color={colours.blue} />
            </TouchableOpacity>
          </View>
          <FlatList
            horizontal
            data={daysInMonth}
            renderItem={renderDay}
            keyExtractor={(item) => item.toISOString()}
            showsHorizontalScrollIndicator={false}
            contentContainerStyle={styles.calendarList}
            initialScrollIndex={
              viewDate.getMonth() === new Date().getMonth() &&
              viewDate.getFullYear() === new Date().getFullYear()
                ? Math.max(0, new Date().getDate() - 3)
                : 0
            }
            getItemLayout={(_, index) => ({
              length: 60 + spacing.small,
              offset: (60 + spacing.small) * index,
              index,
            })}
          />
        </View>

        {showDatePicker && (
          <DateTimePicker
            value={viewDate}
            mode="date"
            display={Platform.OS === 'ios' ? 'spinner' : 'default'}
            onChange={handleDateChange}
            textColor="#fff"
          />
        )}
        {showDatePicker && Platform.OS === 'ios' && (
          <TouchableOpacity
            style={styles.doneButton}
            onPress={() => setShowDatePicker(false)}
          >
            <Body style={styles.doneText}>Done</Body>
          </TouchableOpacity>
        )}

        <View style={styles.searchRow}>
          <View style={styles.searchBar}>
            <Search size={20} color={colours.textMuted} />
            <TextInput
              style={styles.input}
              placeholder="Search user or gear item..."
              placeholderTextColor={colours.textMuted}
              value={search}
              onChangeText={setSearch}
              keyboardAppearance="dark"
            />
          </View>
        </View>

        {isLoading ? (
          <View style={styles.center}>
            <ActivityIndicator color={colours.blue} size="large" />
          </View>
        ) : (
          <FlatList
            data={filteredEntries}
            renderItem={renderLogEntry}
            keyExtractor={(item) => item.id.toString()}
            contentContainerStyle={styles.list}
            refreshControl={
              <RefreshControl
                refreshing={isManualRefreshing}
                onRefresh={onRefresh}
                tintColor={colours.blue}
                colors={[colours.blue]}
              />
            }
            ListEmptyComponent={
              <View style={styles.empty}>
                <Body color={colours.textMuted}>
                  {search
                    ? 'No matching records found'
                    : 'No items returned on this date'}
                </Body>
              </View>
            }
          />
        )}
      </View>
    </BackgroundComponent>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: spacing.medium,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: spacing.medium,
    marginTop: spacing.small,
  },
  searchRow: {
    flexDirection: 'row',
    gap: spacing.small,
    marginBottom: spacing.medium,
  },
  searchBar: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: colours.surface,
    borderRadius: 12,
    paddingHorizontal: spacing.medium,
    height: 50,
    gap: spacing.small,
    borderWidth: 1,
    borderColor: colours.whiteOpacity,
  },
  input: {
    flex: 1,
    color: colours.textPrimary,
    fontSize: 16,
  },
  list: {
    gap: spacing.small,
    paddingBottom: 120,
  },
  logCard: {
    padding: spacing.small,
  },
  logHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
  },
  gearInfo: {
    flex: 1,
  },
  brand: {
    marginBottom: 0,
    fontSize: 16,
  },
  model: {
    fontSize: 12,
    opacity: 0.8,
  },
  tagBadge: {
    backgroundColor: colours.whiteOpacityStrong,
    paddingHorizontal: 6,
    paddingVertical: 2,
    borderRadius: 4,
    borderWidth: 1,
    borderColor: colours.whiteOpacity,
  },
  tagText: {
    color: colours.textPrimary,
    fontWeight: '700',
    fontSize: 9,
  },
  divider: {
    height: 1,
    backgroundColor: colours.whiteOpacity,
    marginVertical: spacing.small,
  },
  logDetails: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    gap: spacing.small,
  },
  detailItem: {
    flex: 1,
    gap: 2,
  },
  detailLabel: {
    fontSize: 9,
    color: colours.textMuted,
    fontWeight: '700',
    letterSpacing: 1,
  },
  detailRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.small,
    marginTop: 0,
  },
  detailValue: {
    fontSize: 12,
    fontWeight: '600',
    color: colours.textPrimary,
  },
  notesRow: {
    marginTop: spacing.medium,
    paddingTop: spacing.small,
    borderTopWidth: 1,
    borderTopColor: colours.whiteOpacity,
    gap: 2,
  },
  notesLabel: {
    fontSize: 9,
    color: colours.textMuted,
    fontWeight: '700',
  },
  notesText: {
    fontSize: 11,
    fontStyle: 'italic',
    color: colours.textSecondary,
  },
  center: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
  empty: {
    alignItems: 'center',
    marginTop: spacing.xxLarge,
  },
  calendarContainer: {
    marginBottom: spacing.medium,
  },
  monthHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: spacing.small,
    paddingHorizontal: spacing.small,
  },
  monthTitle: {
    marginBottom: 0,
  },
  monthTitleButton: {
    paddingHorizontal: spacing.medium,
    paddingVertical: spacing.xSmall,
    borderRadius: 8,
    backgroundColor: colours.whiteOpacity,
  },
  navButton: {
    padding: spacing.small,
    backgroundColor: colours.surface,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: colours.whiteOpacity,
  },
  calendarList: {
    paddingVertical: spacing.small,
    gap: spacing.small,
  },
  dayCard: {
    width: 60,
    height: 80,
    backgroundColor: colours.surface,
    borderRadius: 12,
    alignItems: 'center',
    justifyContent: 'center',
    borderWidth: 1,
    borderColor: colours.whiteOpacity,
    gap: 4,
  },
  dayCardSelected: {
    backgroundColor: colours.blue,
    borderColor: colours.blue,
  },
  dayCardToday: {
    borderColor: colours.blue,
    borderWidth: 1,
  },
  dayName: {
    fontSize: 10,
    color: colours.textMuted,
    fontWeight: '700',
  },
  dayNumber: {
    fontSize: 20,
    marginBottom: 0,
  },
  textSelected: {
    color: '#fff',
  },
  indicator: {
    position: 'absolute',
    bottom: 4,
    backgroundColor: colours.whiteOpacityStrong,
    borderRadius: 10,
    paddingHorizontal: 6,
    paddingVertical: 2,
  },
  indicatorSelected: {
    backgroundColor: 'rgba(255,255,255,0.3)',
  },
  indicatorText: {
    fontSize: 8,
    fontWeight: '800',
    color: '#fff',
  },
  doneButton: {
    alignSelf: 'flex-end',
    padding: spacing.small,
    marginRight: spacing.medium,
  },
  doneText: {
    color: colours.blue,
    fontWeight: '600',
  },
});

export default Logbook;
