   M S B A M L   `   `   `  ����G  CTimeX, Version=1.2.6385.35163, Culture=neutral, PublicKeyToken=null'clr-namespace:HRM.Styles
HRM.Styles  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   DownloadDeviceStatus  � ColumnDefinitions ��Name ��Name �Name � Maximum � Value �� Orientation �� Triggers �� Value	 �� Binding
 �� 
TargetName �� Property �� Value  DownloadingDeviceStatus�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      "localclr-namespace:HRM.Styles  5      %�  &	        &	 �    �� 5      .R�5   
   �   5      �� 5      $	��100 �6#   �� 5	      $��20 �6#   .+�5      �� $ 	txtStatus��6   $��Offline��6+   �� 5      $ 	imgStatus��6   $��16��6'   $��1��63   $+��%pack://application:,,,/Images/Red.png��6C   � 5      $
 pBar��6'   $��2��6   $y�	Collapsed=�65   .+�5      �  6   �� *
LogCount+ 6<   �� *	Counter+�� 5      $��Center=�6   $ 
Horizontal=�6:   .+�5      �� ��6    �� *	Counter+�� 5      $
�� of ��6    �� 5      ��6    �� *
LogCount+�� 5      $�� records done.��6    5      5      5       5   
   �� 5      $ 1��65   	 6   �� *STATUS+.a�5      �� $
 	imgStatus��6    � ��$- 'pack://application:,,,/Images/Green.png��6B   �� 5      $
 	txtStatus��6    � ��$ Online��6@   �� 5      $

 pBar��6    � y�$ 	Collapsed=�6A   5      �� 5      $ 2��65   	 6   �� *STATUS+.a�5      �� $
 	txtStatus��6    � ��$ Connecting...��6@   �� 5      $

 pBar��6    � y�$ 	Collapsed=�6A   5       �� 5!      $ 3��65   	 6   �� *STATUS+.a�5"      �� $
 	txtStatus��6    � ��$ Unauthorized��6@   �� 5#      $

 pBar��6    � y�$ 	Collapsed=�6A   5$      �� 5%      $	 100��65   	 6   �� *STATUS+.a�5&      �� $

 pBar��6    � y�$ Visible=�6A   5'      5)      �� 5+      5-      5/      