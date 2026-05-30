import type {
  GearCategory,
  Sex,
  Size,
  StorageLocation,
} from '@api/common/enums';
import { useSearchGearItems } from '@api/gear-items/gearItemsApi';
import AddGearItemModal from '@components/modals/AddGearItemModal';
import FilterGearModal from '@components/modals/FilterGearModal';
import { useUserContext } from '@contexts/UserContext';
import type { RootStackParamList } from '@navigation/BootRouter';
import {
  type NavigationProp,
  type RouteProp,
  useNavigation,
  useRoute,
} from '@react-navigation/native';
import { borderRadius, colours, fonts, spacing } from '@styles/variables';
import BackgroundComponent from '@ui/BackgroundComponent';
import { Card } from '@ui/Card';
import HeaderComponent from '@ui/HeaderComponent';
import { Body, Heading, Label } from '@ui/Typography';
import { getGearCategoryColor, getGearCategoryLabel } from '@utils/enumHelpers';
import { Plus, Search, SlidersHorizontal } from 'lucide-react-native';
import { useEffect, useMemo, useState } from 'react';
import {
  ActivityIndicator,
  FlatList,
  RefreshControl,
  StyleSheet,
  TextInput,
  TouchableOpacity,
  View,
} from 'react-native';

const Browse = () => {
  const navigation = useNavigation<NavigationProp<RootStackParamList>>();
  const route = useRoute<RouteProp<RootStackParamList, 'Browse'>>();
  const { user } = useUserContext();

  const [search, setSearch] = useState('');
  const [debouncedSearch, setDebouncedSearch] = useState('');
  const [isFilterModalVisible, setIsFilterModalVisible] = useState(false);
  const [isAddModalVisible, setIsAddModalVisible] = useState(false);
  const [filters, setFilters] = useState<{
    gearCategory?: GearCategory;
    sex?: Sex;
    size?: Size;
    storageLocation?: StorageLocation;
  }>({});

  // Apply initial filters from navigation params
  useEffect(() => {
    if (route.params?.storageLocation !== undefined) {
      setFilters((prev) => ({
        ...prev,
        storageLocation: route.params?.storageLocation,
      }));
    }
  }, [route.params?.storageLocation]);

  // Debounce search term to update as you type
  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedSearch(search);
    }, 400); // Slightly longer debounce for better UX

    return () => {
      clearTimeout(handler);
    };
  }, [search]);

  const searchRequest = useMemo(() => ({
    search: debouncedSearch,
    ...filters,
  }), [debouncedSearch, filters]);

  const {
    data: gearItems,
    isLoading,
    isFetching,
    refetch,
  } = useSearchGearItems(searchRequest);

  const isFilterActive = useMemo(() => {
    return Object.values(filters).some((v) => v !== undefined);
  }, [filters]);

  const renderItem = ({ item }: { item: any }) => (
    <TouchableOpacity
      style={styles.cardItem}
      onPress={() => navigation.navigate('GearItem', { id: item.id })}
    >
      <Card style={styles.card}>
        <View style={styles.cardHeader}>
          <View style={styles.cardTitleContainer}>
            <View style={styles.titleBadgeRow}>
              <Body style={styles.brandText}>{item.brand}</Body>
              <View
                style={[
                  styles.categoryBadge,
                  {
                    backgroundColor: `${getGearCategoryColor(item.gearCategory)}20`,
                  },
                ]}
              >
                <Label
                  style={[
                    styles.categoryText,
                    { color: getGearCategoryColor(item.gearCategory) },
                  ]}
                >
                  {getGearCategoryLabel(item.gearCategory)}
                </Label>
              </View>
            </View>
            <Label style={styles.modelText}>
              {item.model || 'Unknown Model'}
            </Label>
          </View>
          <View style={styles.tagBadge}>
            <Label style={styles.tagText}>#{item.toughTag}</Label>
          </View>
        </View>
      </Card>
    </TouchableOpacity>
  );

  return (
    <BackgroundComponent>
      <View style={styles.container}>
        <View style={styles.titleRow}>
          <Heading>Browse Gear</Heading>
          {isFetching && !isLoading && (
            <ActivityIndicator color={colours.blue} size="small" />
          )}
        </View>

        <View style={styles.searchRow}>
          <View style={styles.searchBar}>
            <Search size={20} color={colours.textMuted} />
            <TextInput
              style={styles.input}
              placeholder="Search Gear..."
              placeholderTextColor={colours.textMuted}
              value={search}
              onChangeText={setSearch}
              keyboardAppearance="dark"
            />
          </View>
          <TouchableOpacity
            activeOpacity={0.7}
            style={[
              styles.filterButton,
              isFilterActive && styles.filterButtonActive,
            ]}
            onPress={() => setIsFilterModalVisible(true)}
          >
            <SlidersHorizontal
              size={20}
              color={isFilterActive ? colours.blue : '#fff'}
            />
          </TouchableOpacity>
          {user?.isAdmin && (
            <TouchableOpacity
              activeOpacity={0.7}
              style={styles.addButton}
              onPress={() => {
                console.log('Add Gear button clicked');
                setIsAddModalVisible(true);
              }}
            >
              <Plus size={20} color="#fff" />
            </TouchableOpacity>
          )}
        </View>

        {isLoading ? (
          <View style={styles.center}>
            <ActivityIndicator color={colours.blue} size="large" />
          </View>
        ) : (
          <FlatList
            data={gearItems}
            renderItem={renderItem}
            keyExtractor={(item) => item.id.toString()}
            contentContainerStyle={styles.list}
            numColumns={1}
            refreshControl={
              <RefreshControl
                refreshing={isFetching && !isLoading}
                onRefresh={refetch}
                tintColor={colours.blue}
                colors={[colours.blue]}
              />
            }
            ListEmptyComponent={
              <View style={styles.empty}>
                <Body color={colours.textMuted}>
                  {isFetching ? 'Searching...' : 'No items found'}
                </Body>
              </View>
            }
          />
        )}
      </View>

      <FilterGearModal
        visible={isFilterModalVisible}
        onClose={() => setIsFilterModalVisible(false)}
        filters={filters}
        onApply={setFilters}
      />

      <AddGearItemModal
        visible={isAddModalVisible}
        onClose={() => setIsAddModalVisible(false)}
      />
    </BackgroundComponent>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: spacing.medium,
  },
  titleRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: spacing.small,
  },
  searchRow: {
    flexDirection: 'row',
    gap: spacing.small,
    marginVertical: spacing.medium,
    marginTop: 0,
  },
  searchBar: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: colours.surface,
    borderRadius: borderRadius.medium,
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
    fontFamily: fonts.regular,
  },
  filterButton: {
    width: 50,
    height: 50,
    backgroundColor: colours.surface,
    borderRadius: borderRadius.medium,
    alignItems: 'center',
    justifyContent: 'center',
    borderWidth: 1,
    borderColor: colours.whiteOpacity,
  },
  filterButtonActive: {
    borderColor: colours.blue,
  },
  addButton: {
    width: 50,
    height: 50,
    backgroundColor: colours.blue,
    borderRadius: borderRadius.medium,
    alignItems: 'center',
    justifyContent: 'center',
    borderWidth: 1,
    borderColor: 'rgba(255, 255, 255, 0.2)',
  },
  list: {
    gap: spacing.small,
    paddingBottom: spacing.xxxLarge,
  },
  cardItem: {
    marginBottom: spacing.small,
  },
  card: {
    paddingVertical: spacing.small,
    paddingHorizontal: spacing.medium,
  },
  cardHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.medium,
  },
  cardTitleContainer: {
    flex: 1,
  },
  titleBadgeRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.small,
  },
  brandText: {
    fontWeight: '700',
    color: colours.textPrimary,
  },
  categoryBadge: {
    paddingHorizontal: 6,
    paddingVertical: 2,
    borderRadius: 4,
  },
  categoryText: {
    fontSize: 9,
    fontWeight: '700',
    textTransform: 'uppercase',
  },
  modelText: {
    fontSize: 12,
  },
  tagBadge: {
    backgroundColor: colours.whiteOpacityStrong,
    paddingHorizontal: spacing.small,
    paddingVertical: 4,
    borderRadius: 6,
  },
  tagText: {
    fontSize: 10,
    fontWeight: '700',
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
});

export default Browse;
