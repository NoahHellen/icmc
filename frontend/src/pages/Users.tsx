import { useSearchUsers } from '@api/users/usersApi';
import type { RootStackParamList } from '@navigation/BootRouter';
import { useNavigation } from '@react-navigation/native';
import type { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { borderRadius, colours, fonts, spacing } from '@styles/variables';
import BackgroundComponent from '@ui/BackgroundComponent';
import { Card } from '@ui/Card';
import { Body, Heading, Label } from '@ui/Typography';
import { getMemberTypeLabel } from '@utils/enumHelpers';
import { Search } from 'lucide-react-native';
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

const Users = () => {
  const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();

  const [search, setSearch] = useState('');
  const [debouncedSearch, setDebouncedSearch] = useState('');

  // Debounce search term
  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedSearch(search);
    }, 400);

    return () => {
      clearTimeout(handler);
    };
  }, [search]);

  const searchRequest = useMemo(() => ({
    search: debouncedSearch,
  }), [debouncedSearch]);

  const {
    data: users,
    isLoading,
    isFetching,
    refetch,
  } = useSearchUsers(searchRequest);

  const renderItem = ({ item }: { item: any }) => (
    <TouchableOpacity
      style={styles.cardItem}
      onPress={() => navigation.navigate('User', { id: item.id })}
    >
      <Card style={styles.card}>
        <View style={styles.cardHeader}>
          <View style={styles.cardTitleContainer}>
            <View style={styles.titleBadgeRow}>
              <Body style={styles.nameText}>{item.fullName || 'No Name'}</Body>
              {item.isAdmin && (
                <View style={styles.adminBadge}>
                  <Label style={styles.adminText}>ADMIN</Label>
                </View>
              )}
              <View style={styles.typeBadge}>
                <Label style={styles.typeText}>{getMemberTypeLabel(item.memberType)}</Label>
              </View>
            </View>
            <Label style={styles.emailText}>
              {item.email || 'No Email'}
            </Label>
          </View>
        </View>
      </Card>
    </TouchableOpacity>
  );

  return (
    <BackgroundComponent>
      <View style={styles.container}>
        <View style={styles.titleRow}>
          <Heading>Users</Heading>
          {isFetching && !isLoading && (
            <ActivityIndicator color={colours.blue} size="small" />
          )}
        </View>

        <View style={styles.searchRow}>
          <View style={styles.searchBar}>
            <Search size={20} color={colours.textMuted} />
            <TextInput
              style={styles.input}
              placeholder="Search Users..."
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
            data={users}
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
                  {isFetching ? 'Searching...' : 'No users found'}
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
  nameText: {
    fontWeight: '700',
    color: colours.textPrimary,
    fontSize: 14, // Smaller name
  },
  adminBadge: {
    backgroundColor: `${colours.purple}20`,
    paddingHorizontal: 6,
    paddingVertical: 2,
    borderRadius: 4,
  },
  adminText: {
    fontSize: 8, // Smaller badge text
    fontWeight: '700',
    color: colours.purple,
  },
  emailText: {
    fontSize: 10, // Way smaller email
  },
  typeBadge: {
    backgroundColor: colours.whiteOpacityStrong,
    paddingHorizontal: spacing.small,
    paddingVertical: 4,
    borderRadius: 6,
  },
  typeText: {
    fontSize: 8, // Smaller type text
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

export default Users;
