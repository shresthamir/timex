   M S B A M L   `   `   `  ����>  :HRM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:HRM.Library.CustomControlsHRM.Library.CustomControls  5      a ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes 5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   TextBoxBorder  �� EndPoint � MappingMode �� 
StartPoint � Color � Offset  ComboBoxFocusVisual �� Property �� Value J� SnapsToDevicePixels �� StrokeDashArray  ButtonNormalBackground  ButtonNormalBorder	 �� Color  DownArrowGeometry   ComboBoxReadonlyToggleButton
 �� 
TargetType �� 
TargetType+   %Microsoft.Windows.Themes.ButtonChrome   Name    BorderBrush    
Background J� IsMouseOver    RenderMouseOver    RenderPressed F�Name �� Triggers P� Property P� Value �� 
TargetName  ComboBoxEditableTextBox ��Name  ComboBoxToggleButton    RoundCorners  ComboBoxEditableTemplate2  ,HRM.Library.CustomControls.WatermarkComboBox �Name � ColumnDefinitions �Name � AllowsTransparency �� RelativeSource5  /Microsoft.Windows.Themes.SystemDropShadowChrome  Name   Color  �� MaxDropDownHeight! �� ElementName" ��Name-  'System.Windows.Input.KeyboardNavigation#   DirectionalNavigation,  &Microsoft.Windows.Themes.ListBoxChrome$  Name%   BorderBrush&   BorderThickness'   
Background(   RenderMouseOver) J� IsKeyboardFocusWithin*   RenderFocused+ ��Name, �� 
IsReadOnly-   	Watermark 	 	Watermark. �� Path 
 WatermarkVisibility/ �� 	Converter0 d� 	IsChecked1 �� Mode2 P� 
SourceName3 �� SelectionBoxItemTemplate4 �� SelectionBoxItem#5 �� SelectionBoxItemStringFormat6 �� ContentStringFormat7 J� IsHitTestVisible8 �� Triggers�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    /CC(clr-namespace:HRM.Library.CustomControls  5      UThemesJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  5      :x,http://schemas.microsoft.com/winfx/2006/xaml     5      %5  &	        &	 �     &	 �    &	 �    &	 �    &	 �    7  &	 �    &	 �    7  &	 �
    7  7 	 7 
 7  '  �    7  7  7  7  7   7  �� 5      $
  0,20&�60   $ Absolute=�6@   $	 0,0&�6W   .��5   
   � $ #ABADB3��6   $
 0.05Q�6'   � 5   
   $ #E2E3EA��6   $
 0.07Q�6'   � 5   
   $ #E3E9EF��6   $ 1Q�6'   5	      �� 5
      .�5   
   ��  � �� 5      �� 5      .R�5      �� $��4,4,21,4q�6     . 
���   �$��1��6\   $	 1 2R�6p   5      5      5      �� 5      $	  0,1&�69   $	 0,0&�6H   .��5   
   � $ #F3F3F3��6   $ 0Q�6'   � 5   
   $ #EBEBEB��6   $	 0.5Q�6'   � 5   
   $ #DDDDDD��6   $	 0.5Q�6'   � 5   
   $ #CDCDCD��6   $ 1Q�6'   5      �� 5      $	 	#FF707070��61   �5      M 0 0 L 3.5 4 L 7 0 Z�6@   �� 5      "
 d�61   .�5   
   ��  �@�OverridesDefaultStyle . 62   �� 5   
    � �� .  6&   �� 5   
    � |� .  6&   �� 5   
    �@��	ClickMode$ Press=�6&   �� 5    
    � �� 5!      �� 5"      " d�6"   .R�5#         $ Chrome��6*   # z��6:   # z��6f   # z 6�   # z��6�    . ._�5$      � $��Right=�6   #��� r�6;   .+�5%      F� $ Arrow��6#   8��  
���   �$��Center=�6i   $��3,1,0,0q�6�   $��Center=�6�   5&      5'       5(      P� 5)       �@d�	IsChecked$
 trueY�67   . �5*      �� $ Chrome��6>    �@  RenderPressed . 6R   5+      P� 5,       � {� .  . �5-      �� $ Arrow��65    � �� �#AFAFAF6H   5.      50      52      53      �� 54      "
 ��6,   .�55   
   ��  �@�OverridesDefaultStyle . 62   �� 56   
    �@J�	AllowDrop . 6&   �� 57   
    � ��$ 0��6%   �� 58   
    � ��$ 0��6&   �� 59   
    �@�FocusVisualStyle 6-   Z� �� 5:   
    � �� 5;      �� 5<      " ��6"   .R�5=      �� $ PART_ContentHost��6#   
������ |�.  $��Hidden=�6h   $��Hidden=�6�   5>      5@      5A      �� 5B      "
 d�6)   .�5C   
   ��  �@�OverridesDefaultStyle . 62   �� 5D   
    � �� .  6&   �� 5E   
    � |� .  6&   �� 5F   
    �@��	ClickMode$ Press=�6&   �� 5G   
    � �� 5H      �� 5I      " d�6"   .R�5J         $ Chrome��6*   # z��6:   # z��6f   # z 6�   # z��6�    .   . #��� r�6  ._�5K      F� $ Arrow��6   8��  
���   �$��Center=�6e   $��0,1,0,0q�6�   $��Center=�6�   5L       5M      P� 5N       �@d�	IsChecked$
 trueY�67   . �5O      �� $ Chrome��6>    �@  RenderPressed . 6R   5P      P� 5Q       � {� .  . �5R      �� $ Arrow��65    � �� �#AFAFAF6H   5S      5U      5W      5X      �� 5Y      "  67   .R�5Z   
   � $ 	Placement��6    .  5[      �� 5\      $��* �6#   �� 5]      $
��Auto �6#   .+�5_      � $ 
PART_Popup��6    . $��2��6B   #��� .�6�   $��Bottom=�6  ��6V   �� *IsDropDownOpen+ �� *TemplatedParent+.��5`        $
 Shdw��60   $ Transparent��6>   #��z  6R   ��6�   �� *ActualWidth+$! 	Placement��._�5a      �� $" DropDownBorder��6   #��� ��65   $��1q�6�   #��� ��6�   ._�5b      �� .��5c      �� $# 	Contained=�6-   # z 6b   5d      5e      5f      5g        5h      $$ Border��6#   #% z��63   #& z��6_   #' z��6�   $��2��6�   #( z 6�   #* z) 6  �� 5j      $+ PART_EditableTextBox��6   #��z��64   #��z��6�   8��  #��z��6%  , 6~   �� *
IsReadOnly+ �� *TemplatedParent+6%  �� 5k      #��z- 6   8�� y�6^   �� $! PART_EditableTextBox��$
. Text�8/  d� 5l      $��1��6   8�� 0 6+   �� *IsDropDownOpen+$1 TwoWay=� �� *TemplatedParent+6�   5m       5n   
   P� 5o       �@J�IsKeyboardFocusWithin . . �5p      ��  � ��
 �   �6/   5q      P� 5r       �@��IsDropDownOpen . . �5s      �� $ Border��62    �@ RenderFocused . 6F   5t      P� 5u       �@��HasItems .  . �5v      �� $ DropDownBorder��6+    � ��$ 95��6G   5w      P� 5x       � {� .  . �5y      �� # � ��6/    � ���� 5z       � �� �	#FFF4F4F46/   5{      P� 5|       �@��
IsGrouping . . �5}      ��  � �� .  6B   5~      P� 5      $2 
PART_Popup��6/    �@�HasDropShadow . . �5�      �� $
 Shdw��6+    � ��$ 0,0,5,5q�6=   �� 5�      $
 Shdw��6*    �@ Color$ 	#71000000��6<   5�      5�      �� 5�      "
  6   .�5�   
   ��  �@�FocusVisualStyle8   6-   �� 5�   
    � ��# � ��6'   �� 5�   
    � ��8  6'   �� 5�   
    � ��8  6(   �� 5�   
    � ��$ 1q�6,   �� 5�   
    � ��$
 Auto=�6G   �� 5�   
    � ��$
 Auto=�6E   �� 5�   
    � ��$	 4,3q�6$   �� 5�   
    � ��$ Bottom=�65   �� 5�   
    � �� 5�      �� 5�      "  6"   .R�5�      � $ MainGrid��6    .  5�      �� 5�      $��* �6/   �� 5�      #��� r�6/   $��0 �6�   .+�5�      � $ 
PART_Popup��6     . $��2��6N   $��1q�6�   #��� .�6�   $��Bottom=�6  ��6b   �� *IsDropDownOpen+ �� *TemplatedParent+.��5�        $
 Shdw��6<   $ Transparent��6J   #��z  6^   ��6�   �� *ActualWidth+$! MainGrid��._�5�   "   �� $" DropDownBorder��6)   #��� ��6A   $��1q�6�   #��� ��6�   ._�5�   &   �� ��. .��5�   *   �� $# 	Contained=�69   # z 6n   5�   '   5�   #   5�      5�      d� 5�      #��z��6'   #��z��6S   $��2��6}   8�� 0 6�   �� *IsDropDownOpen+$1 TwoWay=� �� *TemplatedParent+6�   �� 5�      #��z3 6+   #��z��6h   #��z4 6�   #6 z5 6�   #��z��6  7 .  #��z��6w  # z 6�  #��z��6�  5�       5�      P� 5�      $2 
PART_Popup��6;    �@�HasDropShadow . . �5�      �� $
 Shdw��67    � ��$ 0,0,5,5q�6I   �� 5�      $
 Shdw��66    �@ Color$ 	#71000000��6H   5�      P� 5�       �@��HasItems .  . �5�      �� $ DropDownBorder��67    � ��$ 95��6S   5�      P� 5�       � {� .  . �5�      �� # � ��6;    � ���� 5�       � �� �	#FFF4F4F46;   5�      P� 5�       �@��
IsGrouping . . �5�      ��  � �� .  6N   5�      5�      5�      8 5�   
   P� 5�       �@��
IsEditable . . �5�      ��  � ��8  60   �� 5�       � ��# � ��6/   �� 5�       � �� .  6.   �� 5�       � ��$ 3q�6,   �� 5�       � ��8  6-   5�      5�      5�      