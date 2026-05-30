import { GearCategory, Sex, Size, StorageLocation } from '@api/common/enums';
import { useAddGearItem } from '@api/gear-items/gearItemsApi';
import type { AddGearItemRequest } from '@api/gear-items/gearItemsTypes';
import { colours, spacing } from '@styles/variables';
import { Button } from '@ui/Button';
import { DatePicker } from '@ui/DatePicker';
import { Dropdown } from '@ui/Dropdown';
import { Input } from '@ui/Input';
import { Heading } from '@ui/Typography';
import {
  getGearCategoryOptions,
  getSexOptions,
  getSizeOptions,
  getStorageLocationOptions,
} from '@utils/enumHelpers';
import { X } from 'lucide-react-native';
import { useEffect, useState } from 'react';
import {
  Alert,
  Modal,
  ScrollView,
  StyleSheet,
  TouchableOpacity,
  View,
} from 'react-native';

interface AddGearItemModalProps {
  visible: boolean;
  onClose: () => void;
}

const AddGearItemModal = ({ visible, onClose }: AddGearItemModalProps) => {
  const addGearItemMutation = useAddGearItem();

  useEffect(() => {
    console.log('AddGearItemModal visible changed:', visible);
  }, [visible]);

  const getDefaultNextInspection = () => {
    const nextYear = new Date();
    nextYear.setFullYear(nextYear.getFullYear() + 1);
    const year = nextYear.getFullYear();
    const month = String(nextYear.getMonth() + 1).padStart(2, '0');
    const day = String(nextYear.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}T00:00:00Z`;
  };

  const [formData, setFormData] = useState<Partial<AddGearItemRequest>>({
    gearCategory: GearCategory.Carabiner,
    storageLocation: StorageLocation.Imperial,
    nextInspection: getDefaultNextInspection(),
  });

  const handleInputChange = (field: keyof AddGearItemRequest, value: any) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = () => {
    console.log('Submitting gear item:', formData);
    // Validate required fields (based on backend request record)
    if (formData.gearCategory === undefined || formData.storageLocation === undefined) {
      console.warn('Missing required fields:', { 
        gearCategory: formData.gearCategory, 
        storageLocation: formData.storageLocation 
      });
      return;
    }

    addGearItemMutation.mutate(formData as AddGearItemRequest, {
      onSuccess: () => {
        console.log('Gear item added successfully');
        Alert.alert('Success', 'Gear item added successfully');
        onClose();
        // Reset form
        setFormData({
          gearCategory: GearCategory.Carabiner,
          storageLocation: StorageLocation.Imperial,
          nextInspection: getDefaultNextInspection(),
        });
      },
      onError: (error) => {
        console.error('Failed to add gear item:', error);
        Alert.alert('Error', 'Failed to add gear item. Please try again.');
      },
    });
  };

  return (
    <Modal visible={visible} transparent animationType="slide">
      <View style={styles.modalOverlay}>
        <View style={styles.modalContent}>
          <View style={styles.header}>
            <Heading style={styles.title}>Add Gear Item</Heading>
            <TouchableOpacity onPress={onClose} style={styles.closeButton}>
              <X size={24} color={colours.textPrimary} />
            </TouchableOpacity>
          </View>

          <ScrollView style={styles.body} showsVerticalScrollIndicator={false}>
            <Input
              label="Brand"
              placeholder="e.g. Petzl"
              value={formData.brand}
              onChangeText={(text) => handleInputChange('brand', text)}
            />

            <Input
              label="Model"
              placeholder="e.g. Spirit"
              value={formData.model}
              onChangeText={(text) => handleInputChange('model', text)}
            />

            <Input
              label="Tough Tag"
              placeholder="e.g. 1234"
              value={formData.toughTag}
              onChangeText={(text) => handleInputChange('toughTag', text)}
            />

            <Dropdown
              label="Gear Category"
              options={getGearCategoryOptions()}
              selectedValue={formData.gearCategory}
              onValueChange={(value) => handleInputChange('gearCategory', value)}
            />

            <Dropdown
              label="Storage Location"
              options={getStorageLocationOptions()}
              selectedValue={formData.storageLocation}
              onValueChange={(value) =>
                handleInputChange('storageLocation', value)
              }
            />

            <Dropdown
              label="Size"
              options={[{ label: 'N/A', value: undefined }, ...getSizeOptions()]}
              selectedValue={formData.size}
              onValueChange={(value) => handleInputChange('size', value)}
            />

            <Dropdown
              label="Sex"
              options={[{ label: 'Unisex', value: undefined }, ...getSexOptions()]}
              selectedValue={formData.sex}
              onValueChange={(value) => handleInputChange('sex', value)}
            />

            <DatePicker
              label="Date of Purchase"
              value={formData.dateOfPurchase}
              onChange={(date) => handleInputChange('dateOfPurchase', date)}
            />

            <DatePicker
              label="Next Inspection"
              value={formData.nextInspection}
              onChange={(date) => handleInputChange('nextInspection', date)}
            />

            <Input
              label="Length (m)"
              placeholder="e.g. 60"
              value={formData.length?.toString()}
              onChangeText={(text) =>
                handleInputChange('length', text ? Number.parseInt(text) : undefined)
              }
              keyboardType="numeric"
            />
          </ScrollView>

          <View style={styles.footer}>
            <Button
              title="Cancel"
              onPress={onClose}
              variant="ghost"
              style={styles.cancelButton}
            />
            <Button
              title={addGearItemMutation.isPending ? 'Adding...' : 'Add Item'}
              onPress={handleSubmit}
              disabled={addGearItemMutation.isPending}
              style={styles.addButton}
            />
          </View>
        </View>
      </View>
    </Modal>
  );
};

const styles = StyleSheet.create({
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0, 0, 0, 0.8)',
    justifyContent: 'center',
    padding: spacing.large,
  },
  modalContent: {
    backgroundColor: colours.background,
    borderRadius: 28,
    padding: spacing.large,
    maxHeight: '90%',
    borderWidth: 1,
    borderColor: colours.whiteOpacityStrong,
    elevation: 20,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 10 },
    shadowOpacity: 0.5,
    shadowRadius: 20,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: spacing.large,
  },
  title: {
    fontSize: 24,
  },
  closeButton: {
    padding: spacing.small,
  },
  body: {
    marginBottom: spacing.large,
  },
  footer: {
    flexDirection: 'row',
    gap: spacing.medium,
  },
  cancelButton: {
    flex: 1,
  },
  addButton: {
    flex: 1,
  },
});

export default AddGearItemModal;
