export interface CustomizationGroup {
  id: string;
  productId: string;
  name: string;
  type: 'SingleChoice' | 'MultipleChoice';
  minSelections: number;
  maxSelections: number;
  required: boolean;
  position: number;
  options: CustomizationOption[];
}

export interface CustomizationOption {
  id: string;
  groupId: string;
  name: string;
  priceModifier: number;
  position: number;
}

export interface SaveCustomizationGroupRequest {
  id?: string;
  name: string;
  type: 'SingleChoice' | 'MultipleChoice';
  minSelections: number;
  maxSelections: number;
  required: boolean;
  position: number;
  options: SaveCustomizationOptionRequest[];
}

export interface SaveCustomizationOptionRequest {
  id?: string;
  name: string;
  priceModifier: number;
  position: number;
}

export interface SelectedOption {
  optionId: string;
  optionName: string;
  groupName: string;
  priceModifier: number;
}
