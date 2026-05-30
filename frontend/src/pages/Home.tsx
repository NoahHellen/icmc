import { StorageLocation } from '@api/common/enums';
import { useSearchGearItems } from '@api/gear-items/gearItemsApi';
import type { GearItemDto } from '@api/gear-items/gearItemsTypes';
import { useUserContext } from '@contexts/UserContext';
import type { RootStackParamList } from '@navigation/BootRouter';
import { type NavigationProp, useNavigation } from '@react-navigation/native';
import { borderRadius, colours, spacing } from '@styles/variables';
import BackgroundComponent from '@ui/BackgroundComponent';
import { Card } from '@ui/Card';
import { Body, Heading, Label, Subheading } from '@ui/Typography';
import { formatDate, getDueDateLabel } from '@utils/enumHelpers';
import {
  AlertCircle,
  Calendar,
  ChevronRight,
  History,
  Info,
  MapPin,
  Package,
  User as UserIcon,
} from 'lucide-react-native';
import { useMemo, useState } from 'react';
import {
  ActivityIndicator,
  RefreshControl,
  ScrollView,
  StyleSheet,
  TouchableOpacity,
  View,
} from 'react-native';

const Home = () => {
  const { user } = useUserContext();
  const navigation = useNavigation<NavigationProp<RootStackParamList>>();
  const searchRequest = useMemo(() => ({}), []);
  const {
    data: allItems,
    isLoading,
    isFetching,
    refetch,
  } = useSearchGearItems(searchRequest);
  const [isManualRefreshing, setIsManualRefreshing] = useState(false);

  const onRefresh = async () => {
    setIsManualRefreshing(true);
    await refetch();
    setIsManualRefreshing(false);
  };

  const processedData = useMemo(() => {
    if (!allItems)
      return {
        userLoans: [],
        imperial: { total: 0, available: 0, needsInspection: 0 },
        westway: { total: 0, available: 0, needsInspection: 0 },
      };

    return allItems.reduce(
      (acc, item) => {
        if (item.lentToUserId === user?.id && !item.returnedDate) {
          acc.userLoans.push(item);
        }

        let needsInspection = false;
        if (item.nextInspection) {
          const diff =
            new Date(item.nextInspection).getTime() - new Date().getTime();
          if (diff < 0) needsInspection = true;
        }

        if (item.storageLocation === StorageLocation.Imperial) {
          acc.imperial.total++;
          if (!item.lentToUserId) acc.imperial.available++;
          if (needsInspection) acc.imperial.needsInspection++;
        } else if (item.storageLocation === StorageLocation.Westway) {
          acc.westway.total++;
          if (!item.lentToUserId) acc.westway.available++;
          if (needsInspection) acc.westway.needsInspection++;
        }

        return acc;
      },
      {
        userLoans: [] as GearItemDto[],
        imperial: { total: 0, available: 0, needsInspection: 0 },
        westway: { total: 0, available: 0, needsInspection: 0 },
      }
    );
  }, [allItems, user?.id]);

  const navigateToBrowse = (location: StorageLocation) => {
    navigation.navigate('Main', {
      screen: 'Browse',
      params: { storageLocation: location },
    });
  };

  return (
    <BackgroundComponent>
      <ScrollView
        contentContainerStyle={styles.container}
        refreshControl={
          <RefreshControl
            refreshing={isManualRefreshing}
            onRefresh={onRefresh}
            tintColor={colours.blue}
            colors={[colours.blue]}
          />
        }
      >
        <View style={styles.headerRow}>
          <View style={styles.welcome}>
            <Heading style={styles.title}>
              Hello, {user?.fullName?.split(' ')[0] || 'Guest'}
            </Heading>
            <Body color={colours.textSecondary}>
              Welcome back to ICMC Inventory.
            </Body>
          </View>
          <TouchableOpacity
            style={styles.accountButton}
            onPress={() => navigation.navigate('Account')}
            activeOpacity={0.7}
          >
            <UserIcon size={18} color="#fff" />
            <Label style={styles.accountButtonText}>Account</Label>
          </TouchableOpacity>
        </View>

        <Subheading style={styles.sectionTitle}>Your Loans</Subheading>
        <Card style={styles.loansCard}>
          {isLoading ? (
            <ActivityIndicator color={colours.blue} />
          ) : processedData.userLoans.length > 0 ? (
            <View style={styles.loansList}>
              {processedData.userLoans.map((item) => (
                <View key={item.id} style={styles.loanItem}>
                  <View style={styles.loanItemMain}>
                    <Package size={16} color={colours.blue} />
                    <Body style={styles.loanItemText}>
                      {item.brand} {item.model}
                    </Body>
                    <Label style={styles.tagText}>#{item.toughTag}</Label>
                  </View>
                  <View style={styles.loanDueDate}>
                    <Calendar size={12} color={colours.pink} />
                    <Label
                      color={
                        getDueDateLabel(item.expectedReturnDate).isOverdue
                          ? colours.pink
                          : colours.textSecondary
                      }
                    >
                      {getDueDateLabel(item.expectedReturnDate).isOverdue
                        ? 'Overdue'
                        : `Due ${formatDate(item.expectedReturnDate)}`}
                    </Label>
                  </View>
                </View>
              ))}
            </View>
          ) : (
            <Body color={colours.textMuted} style={styles.emptyText}>
              You currently have no items on loan.
            </Body>
          )}
        </Card>

        <Subheading style={styles.sectionTitle}>Storage Locations</Subheading>
        <View style={styles.storageGrid}>
          <TouchableOpacity
            style={styles.storageItem}
            onPress={() => navigateToBrowse(StorageLocation.Imperial)}
          >
            <Card style={styles.storageCard}>
              <View style={styles.storageHeader}>
                <MapPin size={18} color={colours.blue} />
                <Body style={styles.storageName}>Imperial</Body>
              </View>
              <View style={styles.storageStats}>
                <Body style={styles.compactStat}>
                  <Body style={styles.statHighlight}>
                    {processedData.imperial.available}
                  </Body>
                  /{processedData.imperial.total} available
                </Body>
                {processedData.imperial.needsInspection > 0 && (
                  <View style={styles.alertLine}>
                    <AlertCircle size={12} color={colours.pink} />
                    <Label color={colours.pink}>
                      {processedData.imperial.needsInspection} to inspect
                    </Label>
                  </View>
                )}
              </View>
            </Card>
          </TouchableOpacity>

          <TouchableOpacity
            style={styles.storageItem}
            onPress={() => navigateToBrowse(StorageLocation.Westway)}
          >
            <Card style={styles.storageCard}>
              <View style={styles.storageHeader}>
                <MapPin size={18} color={colours.purple} />
                <Body style={styles.storageName}>Westway</Body>
              </View>
              <View style={styles.storageStats}>
                <Body style={styles.compactStat}>
                  <Body style={styles.statHighlight}>
                    {processedData.westway.available}
                  </Body>
                  /{processedData.westway.total} available
                </Body>
                {processedData.westway.needsInspection > 0 && (
                  <View style={styles.alertLine}>
                    <AlertCircle size={12} color={colours.pink} />
                    <Label color={colours.pink}>
                      {processedData.westway.needsInspection} to inspect
                    </Label>
                  </View>
                )}
              </View>
            </Card>
          </TouchableOpacity>
        </View>

        <Card style={styles.infoCard}>
          <View style={styles.infoContent}>
            <Info size={18} color={colours.blue} />
            <Body style={styles.infoText}>
              The ICMC committee are not licensed professionals. Please inspect
              gear you borrow before use.
            </Body>
          </View>
        </Card>
      </ScrollView>
    </BackgroundComponent>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: spacing.medium,
    paddingBottom: spacing.xxxLarge,
  },
  welcome: {
    flex: 1,
  },
  headerRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginTop: spacing.medium,
    marginBottom: spacing.large,
  },
  accountButton: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 6,
    backgroundColor: colours.whiteOpacityStrong,
    paddingHorizontal: spacing.medium,
    paddingVertical: spacing.small,
    borderRadius: borderRadius.medium,
    borderWidth: 1,
    borderColor: colours.whiteOpacityStrong,
  },
  accountButtonText: {
    fontWeight: '700',
    fontSize: 10,
    textTransform: 'uppercase',
    letterSpacing: 1,
  },
  title: {
    fontSize: 28,
    marginBottom: 2,
  },
  sectionTitle: {
    marginBottom: spacing.small,
    fontSize: 12,
    color: colours.textMuted,
    textTransform: 'uppercase',
    letterSpacing: 1,
    fontWeight: '700',
  },
  loansCard: {
    marginBottom: spacing.large,
    padding: spacing.medium,
    backgroundColor: colours.surface,
  },
  loansList: {
    gap: spacing.small,
  },
  loanItem: {
    gap: spacing.xxSmall,
  },
  loanItemMain: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.small,
  },
  loanDueDate: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xSmall,
    marginLeft: 22, // 16 icon + 6 gap
  },
  loanItemText: {
    flex: 1,
    fontSize: 14,
  },
  tagText: {
    fontSize: 10,
    backgroundColor: colours.whiteOpacity,
    paddingHorizontal: 6,
    paddingVertical: 2,
    borderRadius: 4,
  },
  emptyText: {
    textAlign: 'center',
    fontStyle: 'italic',
    paddingVertical: spacing.small,
  },
  storageGrid: {
    flexDirection: 'row',
    gap: spacing.small,
    marginBottom: spacing.large,
  },
  storageItem: {
    flex: 1,
  },
  storageCard: {
    padding: spacing.small,
    backgroundColor: colours.surface,
    minHeight: 100,
    justifyContent: 'space-between',
  },
  storageHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 6,
    marginBottom: spacing.xSmall,
  },
  storageName: {
    fontSize: 14,
    fontWeight: '700',
  },
  storageStats: {
    gap: 2,
  },
  compactStat: {
    fontSize: 11,
    color: colours.textSecondary,
  },
  statHighlight: {
    fontWeight: '700',
    color: colours.textPrimary,
  },
  alertLine: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 4,
    marginTop: 2,
  },
  infoCard: {
    marginTop: spacing.small,
    backgroundColor: colours.surfaceLight,
    borderLeftWidth: 3,
    borderLeftColor: colours.blue,
  },
  infoContent: {
    flexDirection: 'row',
    gap: spacing.medium,
    alignItems: 'flex-start',
  },
  infoText: {
    flex: 1,
    fontSize: 13,
    lineHeight: 18,
    color: colours.textSecondary,
  },
});

export default Home;
