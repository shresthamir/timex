   M S B A M L   `   `   `  ����	  TimeX    HRM.UI.Misc.Splash   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Cee4http://schemas.microsoft.com/expression/2010/effects     5   	   Hei9http://schemas.microsoft.com/expression/2010/interactions     5   	     � WindowStyle$
  None=�5   	    � 
ResizeMode$ NoResize=�6   $	��235��6H   $	��500��6U    � WindowStartupLocation$ CenterScreen=�6a    � Title$ Splash��5   	   ��5   2   Z�  � 	Resources 5	       
  FadeIn �� TargetProperty �� 
TargetName9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value  StartProgress��%�  &	        &	 �     �� 5
   
   .��5      U� $ (UIElement.Opacity)�6,   $
 grid��6\   .\�5        $ 0��6'   $ 0Q�63     5      $ 0:0:2��6'   $ 1Q�67   5      5      �� 5   
   .��5      U� $ (ProgressBar.Value)�6,   $ Progress��6\   .\�5        $ 0��6'   $ 0Q�63     5      $ 0:0:3��6'   $	 100Q�67   5      5      5      	 � Triggers	 5      :� 5   
   
 :� RoutedEvent$
 FrameworkElement.Loaded��6   .[�5      �� #��[  6   �� 5      #��[ 6   5      .��5       � -   5         �Name$
 grid��5        � RowDefinitions 5!   
   �� 5"      �� 5#      $��25 �6   .+�5%   
   �� -   5         ��Name$	 img��5%      $2��,pack://application:,,,/Images/TimeX Logo.jpg��6   $	��325��6S   � 5&   
   -   5         �Name$ Progress��5&      $��1��6(   ���#34495e6L   �� 5'   
   $��1��6   $��Center=�6p   $��Center=�6�   ��6!   ��  �� ElementName$ Progress�� �� Path$ Value� �� StringFormat$ Loading-{0:#0}%��6�   5(      5*      