   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5   	   $    Client.AccountManagementWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Client     5   	   -   5          � Title$  Account Management��5   	   $	��540��6$   $	��960��61    � WindowStartupLocation$ CenterScreen=�6=    � 
ResizeMode$ CanMinimize=�6b   .��5	      � $��0,0,-0.4,-4q�6   .+�5   
   �� $��Account Information��6   $
��Left=�60   $	��115��6K   $��
39,105,0,0q�6X   $	��Top=�6l   $	��359��6�   $��14(�6�   ���	#FFD5DFE56�   �� 5   
   $��Select an Account��6   $
��Left=�6.   $��88��6I   $	��Top=�6U   $	��245��6m   $��14(�6y   ���	#FFD5DFE5$��
668,77,0,0q�6�   �� 5   
   -   5         ��Name$! displayUserAccountsComboBox��5      $
��Left=�68   $	��Top=�6S   $	��217��6k   $��26��6w   $��683,119,0,0q�6�    � Cursor$
 Hand��6�    � ToolTip$% Click to Select a Bank Account!��6�   �� 5   
   $��Withdraw��6   $
��Left=�6%   $	��132��6@   $	��Top=�6M   $	��284��6e   $��14(�6q   $��634,343,0,0q�6   ���	#FFD5DFE56�   �� 5   
   -   5         ��Name$ withdrawAmountDollar��5      $
��Left=�60   $��19��6K    �� TextWrapping$
 Wrap=�6W   $	��Top=�6k   $	��146��6�   $��754,387,0,0q�6�   �� 5   
   $��$��6   $
��Left=�6   $	��Top=�67   $��16��6O   $��740,382,0,0q�6Z   �� 5   
   $��Insert Amount��6   $
��Left=�6(   $	��Top=�6C   $��88��6[   $��652,382,0,0q�6f   �� 5   
   -   5         ��Name$ withdrawButton��5      $��Withdraw from Account��6)   $
��Left=�6I   $	��Top=�6d   $	��248��6|   $��36��6�   $��652,420,0,0q�6�   ���	#FF62C783
�������
�������$
 Hand��6  $ Click to Withdraw!��6  �� 5   
   -   5        $ goToMainWindowButton��5      $��< Go to Main Window��6/   $
��Left=�6P   $��	19,19,0,0q�6k   $	��Top=�6~   $	��159��6�   $��26��6�   ���	#FF62C783$
 Hand��6�   $% Click to go to the Main Window!��6�   ��5      �� 5      	 �� Color$	 	#FF62C783��6"   
 �� Opacity$

 0.01Q�64   5      �� 5   
   -   5         ��Name$( "accountManagementUserGreetingLabel��5      $��!User Greeting��6<   $
��Left=�6U   $��
748,10,0,0q�6p   $	��Top=�6�   $	��196��6�   $��RightToLeft=�6�   $��13(�6�   �� 5   
   $��Deposit��6   $
��Left=�6$   $	��132��6?   $	��Top=�6L   $	��292��6d   $��14(�6p   $��
41,342,0,0q�6~   ���	#FFD5DFE56�   �� 5   
   -   5        $ depositAmountDollar��5      $
��Left=�6/   $��20��6J   $
 Wrap=�6V   $	��Top=�6j   $	��141��6�   $��162,381,0,0q�6�    �� MaxLines$ 1��6�   �� 5   
   $��$��6   $
��Left=�6   $	��Top=�67   $��23��6O    J� RenderTransformOrigin$ 0.779,0.867&�6Z   $��148,377,0,0q�6~   �� 5   
   $��Insert Amount��6   $
��Left=�6(   $	��Top=�6C   $��87��6[   $��29��6f   $��
56,377,0,0q�6r   �� 5   
   -   5        $ depositButton��5      $��Deposit to Account��6(   $
��Left=�6E   $	��Top=�6`   $	��247��6x   $��36��6�   $��
56,420,0,0q�6�   ���	#FF62C783
�������
�������$
 Hand��6�   $ Click to Deposit!��6  �� 5   
   -	   5        $  displayAccountBalanceLabel��5      $-��'Account Balance : (No Account Selected)��64   $
��Left=�6f   $	��Top=�6�   $	��322��6�   $��16(�6�   $��
56,133,0,0q�6�   �� 5    
   -
   5        $ displayAccountNumberLabel��5       $2��,Account ID : (Select an Account to Proceed!)��63   $
��Left=�6j   $	��Top=�6�   $	��322��6�   $��14(�6�   $��
56,176,0,0q�6�   �� 5!   
   $��Create an Account��6   $
��Left=�6.   $��86��6I   $	��Top=�6U   $	��245��6m   $��14(�6y   ���	#FFD5DFE5$��667,194,0,0q�6�   �� 5"   
   -   5        $ createAccountButton��5"      $��Create Account��6.   $
��Left=�6G   $	��Top=�6b   $	��217��6z   $��34��6�   ���	#FF62C783
�������
�������$��683,230,0,0q�6�   $
 Hand��6  $% Click to Create a Bank Account!��6  �� 5#   
   $
��Left=�6   $	��958��62   $��475,-177,0,-266.6q�6?   $	��Top=�6Z   $��5��6r   $ 0.5,0.5&�6|   {�.  ���	#FF151313��5$      �� 5%      $	 	#FF86E4BD��6"   $	
 0.2Q�64   z�5'      V� 5(      .~�5)      �� �� 5*      �� 5+       �� Angle$ 90Q�6&   T� 5,      5-      5/      51      52      