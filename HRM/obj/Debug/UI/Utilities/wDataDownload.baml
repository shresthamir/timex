   M S B A M L   `   `   `  ����  HRM$    HRM.UI.Utilities.wDataDownload   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � 
ResizeMode$  NoResize=�5   	    � Title$ Download Attendance Data��5   	   $	��300��6*   $	��550��67   .��5      ��    PlainBackground#��[  6    �� Padding$ 5q�66   ._�5   
   �  � RowDefinitions 5      �� 5	      $��25 �6    �� 5
      �� 5      $��40 �6    �� 5      $��40 �6    .+�5      �� $$��Download Data from all devices��6    d� 	IsChecked 6@   �� *	AllDevice+&   System.Windows.Controls.DataGrid  5      $��1��6   $
��Auto=�6�   ��6$   �� *
DeviceList+{�6G   �� *	AllDevice+  
InvertBool �� 	Converter# [    Columns 5      4  .System.Windows.Controls.DataGridCheckBoxColumn  5      ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ 25 6L   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6-   �� *
IsSelected+6L   0  *System.Windows.Controls.DataGridTextColumn  5      	   Header$	 Model��6)   $	 120 6Y    68   �� *DEVICE_MODEL+6Y     5      $
	 Name��6)   $	 200 6X    67   �� *DISPLAY_NAME+6X     5      $	 
IP Address��6)   $	 120 6[    6=   �� *	DEVICE_IP+6[   4  .System.Windows.Controls.DataGridTemplateColumn  5      $ 40 6-   
   CellTemplate
 5      �� 5      .R�5   "   ��  ��Name$ 	imgStatus��6(   $��16��6;   $+��%pack://application:,,,/Images/Red.png��6G    �� Triggers 5   "   �� 5   &    �� Value$
 true��6M    �� Binding 62   �� *STATUS+.a�5   *   ��  �� 
TargetName$ 	imgStatus��61    �� Property � �� �� Value$- 'pack://application:,,,/Images/Green.png��6Z   5   '   5      5!      5#      �� 5%      $��Download��6   $��2��6(   �� 5&      $��Refresh��6   $��3��6'   5(      5)      5*      