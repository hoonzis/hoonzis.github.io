---
layout: post
title: Silverlight Accordion - Changing the Header Style
date: '2011-02-19T03:09:00.001-08:00'
author: Jan Fajfr
tags:
- Silverlight
modified_time: '2014-06-27T05:10:20.756-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-6603543459515201982
blogger_orig_url: http://hoonzis.blogspot.com/2011/02/silverlight-accordion-changing-header.html
---

Changing the header style for all items in the Siverlight Toolkit's Accordion item is not that easy as you might think it would be. If you wish to change the Container for each Accordion Item, you can just change the **ItemContainerStyle** property, however this will not help you to change the header. You can also apply some changes directly to the Header property of each Item that you add to the accordion.

```xml
<layoutToolkit:Accordion>
    <layoutToolkit:Accordion.ItemContainerStyle>
           <style TargetType="layoutToolkit:AccordionItem">
               <setter Property="BorderBrush" Value="Black"/>
               <setter Property="BorderThickness" Value="1"/>
               <setter Property="Background" Value="DarkRed"/>
           </Style>
    </layoutToolkit:Accordion.ItemContainerStyle>
</laoutToolkit:Accordion>
```

But the problem here is that the Border component never stretches across the whole header and besides you would have to add this to each Item (or change the Item Template).

### Changing the AccordionButtonStyle
The solution here is to override the AccordionButtonStyle Property of the Accordion. To have some base you can get the style which is presented in the Silverlight Toolkit. To locate here go to **"Siverlight Toolkits Source\\Controls.Layout.Toolkit\\Themes\\generic.xaml"**.

Here is the style and you can see the changes made to change the background of the Item Header.

```xml
<style TargetType="layoutPrimitivesToolkit:AccordionButton" x:Key="MyAccButtonStyle">
        <setter Property="BorderThickness" Value="0"/>
        <setter Property="Background" Value="White"/>
        <setter Property="HorizontalAlignment" Value="Stretch"/>
        <setter Property="VerticalAlignment" Value="Stretch"/>
        <setter Property="HorizontalContentAlignment" Value="Center"/>
        <setter Property="VerticalContentAlignment" Value="Center"/>
        <setter Property="IsTabStop" Value="True"/>
        <setter Property="TabNavigation" Value="Once"/>
        <setter Property="Template">
            <Setter.Value>
                <controltemplate TargetType="layoutPrimitivesToolkit:AccordionButton">
                    <grid Margin="{TemplateBinding Padding}" Background="Transparent">
                        <vsm:VisualStateManager.VisualStateGroups>
                            <vsm:VisualStateGroup x:Name="ExpandDirectionStates">
                                <vsm:VisualStateGroup.Transitions>
                                    <vsm:VisualTransition GeneratedDuration="0"/>
                                </vsm:VisualStateGroup.Transitions>
                                <vsm:VisualState x:Name="ExpandDown">
                                    <storyboard>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="icon" Storyboard.TargetProperty="(Grid.Column)">
                                            <discreteobjectkeyframe KeyTime="0" Value="0"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="icon" Storyboard.TargetProperty="(Grid.Row)">
                                            <discreteobjectkeyframe KeyTime="0" Value="0"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="header" Storyboard.TargetProperty="(Grid.Column)">
                                            <discreteobjectkeyframe KeyTime="0" Value="1"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="header" Storyboard.TargetProperty="(Grid.Row)">
                                            <discreteobjectkeyframe KeyTime="0" Value="0"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="cd0" Storyboard.TargetProperty="Width">
                                            <discreteobjectkeyframe KeyTime="0" Value="Auto"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="cd1" Storyboard.TargetProperty="Width">
                                            <discreteobjectkeyframe KeyTime="0" Value="*"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <doubleanimation BeginTime="00:00:00" Duration="00:00:00" Storyboard.TargetName="arrow" Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)" To="-90"/>
                                    </Storyboard>
                                </vsm:VisualState>
                                <vsm:VisualState x:Name="ExpandUp">
                                    <storyboard>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="icon" Storyboard.TargetProperty="(Grid.Column)">
                                            <discreteobjectkeyframe KeyTime="0" Value="1"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="icon" Storyboard.TargetProperty="(Grid.Row)">
                                            <discreteobjectkeyframe KeyTime="0" Value="1"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="header" Storyboard.TargetProperty="(Grid.Column)">
                                            <discreteobjectkeyframe KeyTime="0" Value="0"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="header" Storyboard.TargetProperty="(Grid.Row)">
                                            <discreteobjectkeyframe KeyTime="0" Value="1"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="cd0" Storyboard.TargetProperty="Width">
                                            <discreteobjectkeyframe KeyTime="0" Value="*"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="cd1" Storyboard.TargetProperty="Width">
                                            <discreteobjectkeyframe KeyTime="0" Value="Auto"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <doubleanimation BeginTime="00:00:00" Duration="00:00:00" Storyboard.TargetName="arrow" Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)" To="90"/>
                                    </Storyboard>
                                </vsm:VisualState>
                                <vsm:VisualState x:Name="ExpandLeft">
                                    <storyboard>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="icon" Storyboard.TargetProperty="(Grid.Column)">
                                            <discreteobjectkeyframe KeyTime="0" Value="1"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="icon" Storyboard.TargetProperty="(Grid.Row)">
                                            <discreteobjectkeyframe KeyTime="0" Value="0"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="header" Storyboard.TargetProperty="(Grid.Column)">
                                            <discreteobjectkeyframe KeyTime="0" Value="1"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="header" Storyboard.TargetProperty="(Grid.Row)">
                                            <discreteobjectkeyframe KeyTime="0" Value="1"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="rd0" Storyboard.TargetProperty="Height">
                                            <discreteobjectkeyframe KeyTime="0" Value="Auto"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="rd1" Storyboard.TargetProperty="Height">
                                            <discreteobjectkeyframe KeyTime="0" Value="*"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="header" Storyboard.TargetProperty="LayoutTransform">
                                            <discreteobjectkeyframe KeyTime="0">
                                                <DiscreteObjectKeyFrame.Value>
                                                    <transformgroup>
                                                        <rotatetransform Angle="90"/>
                                                    </TransformGroup>
                                                </DiscreteObjectKeyFrame.Value>
                                            </DiscreteObjectKeyFrame>
                                        </ObjectAnimationUsingKeyFrames>
                                        <doubleanimation BeginTime="00:00:00" Duration="00:00:00" Storyboard.TargetName="arrow" Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)" To="0"/>
                                    </Storyboard>
                                </vsm:VisualState>
                                <vsm:VisualState x:Name="ExpandRight">
                                    <storyboard>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="icon" Storyboard.TargetProperty="(Grid.Column)">
                                            <discreteobjectkeyframe KeyTime="0" Value="0"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="icon" Storyboard.TargetProperty="(Grid.Row)">
                                            <discreteobjectkeyframe KeyTime="0" Value="1"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="header" Storyboard.TargetProperty="(Grid.Column)">
                                            <discreteobjectkeyframe KeyTime="0" Value="0"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="header" Storyboard.TargetProperty="(Grid.Row)">
                                            <discreteobjectkeyframe KeyTime="0" Value="0"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="rd0" Storyboard.TargetProperty="Height">
                                            <discreteobjectkeyframe KeyTime="0" Value="*"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="rd1" Storyboard.TargetProperty="Height">
                                            <discreteobjectkeyframe KeyTime="0" Value="Auto"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <objectanimationusingkeyframes BeginTime="0" Duration="0" Storyboard.TargetName="header" Storyboard.TargetProperty="LayoutTransform">
                                            <discreteobjectkeyframe KeyTime="0">
                                                <DiscreteObjectKeyFrame.Value>
                                                    <transformgroup>
                                                        <rotatetransform Angle="-90"/>
                                                    </TransformGroup>
                                                </DiscreteObjectKeyFrame.Value>
                                            </DiscreteObjectKeyFrame>
                                        </ObjectAnimationUsingKeyFrames>
                                        <doubleanimation BeginTime="00:00:00" Duration="00:00:00" Storyboard.TargetName="arrow" Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)" To="180"/>
                                    </Storyboard>
                                </vsm:VisualState>
                            </vsm:VisualStateGroup>
                            <vsm:VisualStateGroup x:Name="ExpansionStates">
                                <vsm:VisualStateGroup.Transitions>
                                    <vsm:VisualTransition GeneratedDuration="0"/>
                                </vsm:VisualStateGroup.Transitions>
                                <vsm:VisualState x:Name="Collapsed">
                                    <storyboard>
                                        <doubleanimation BeginTime="00:00:00" Duration="00:00:00.3" Storyboard.TargetName="icon" Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)" To="0"/>
                                    </Storyboard>
                                </vsm:VisualState>
                                <vsm:VisualState x:Name="Expanded">
                                    <storyboard>
                                        <doubleanimation BeginTime="00:00:00" Duration="00:00:00.3" Storyboard.TargetName="icon" Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)" To="90"/>
                                        <coloranimationusingkeyframes BeginTime="00:00:00" Duration="00:00:00.0010000" Storyboard.TargetName="ExpandedBackground" Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)">
                                            <!-- ********************* -->
                                            <!-- Expended Item  Header -->
                                            <!-- ********************* -->
                                            <splinecolorkeyframe KeyTime="00:00:00" Value="#1BA1E2"/>
                                        </ColorAnimationUsingKeyFrames>
                                        <doubleanimationusingkeyframes BeginTime="00:00:00" Duration="00:00:00.0010000" Storyboard.TargetName="ExpandedBackground" Storyboard.TargetProperty="(UIElement.Opacity)">
                                            <splinedoublekeyframe KeyTime="00:00:00" Value="1"/>
                                        </DoubleAnimationUsingKeyFrames>
                                    </Storyboard>
                                </vsm:VisualState>
                            </vsm:VisualStateGroup>
                            <vsm:VisualStateGroup x:Name="CheckStates">
                                <vsm:VisualStateGroup.Transitions>
                                    <vsm:VisualTransition GeneratedDuration="00:00:00"/>
                                </vsm:VisualStateGroup.Transitions>
                                <vsm:VisualState x:Name="Checked"/>
                                <vsm:VisualState x:Name="Unchecked"/>
                            </vsm:VisualStateGroup>
                            <vsm:VisualStateGroup x:Name="CommonStates">
                                <vsm:VisualStateGroup.Transitions>
                                    <vsm:VisualTransition GeneratedDuration="0"/>
                                    <vsm:VisualTransition From="MouseOver" GeneratedDuration="00:00:00.1" To="Normal"/>
                                    <vsm:VisualTransition GeneratedDuration="00:00:00.1" To="MouseOver"/>
                                    <vsm:VisualTransition GeneratedDuration="00:00:00.1" To="Pressed"/>
                                </vsm:VisualStateGroup.Transitions>
                                <vsm:VisualState x:Name="Normal"/>
                                <vsm:VisualState x:Name="MouseOver">
                                    <storyboard>
                                        <coloranimation BeginTime="0" Storyboard.TargetName="arrow" Storyboard.TargetProperty="(Path.Stroke).(SolidColorBrush.Color)" To="#222"/>
                                        <coloranimationusingkeyframes BeginTime="00:00:00" Duration="00:00:00.0010000" Storyboard.TargetName="MouseOverBackground" Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)">
                                            <!-- *********************** -->
                                            <!-- Mouse Over Item  Header -->
                                            <!-- *********************** -->
                                            <splinecolorkeyframe KeyTime="00:00:00" Value="#E51400"/>
                                        </ColorAnimationUsingKeyFrames>
                                        <doubleanimationusingkeyframes BeginTime="00:00:00" Duration="00:00:00.0010000" Storyboard.TargetName="MouseOverBackground" Storyboard.TargetProperty="(UIElement.Opacity)">
                                            <splinedoublekeyframe KeyTime="00:00:00" Value="1"/>
                                        </DoubleAnimationUsingKeyFrames>
                                    </Storyboard>
                                </vsm:VisualState>
                                <vsm:VisualState x:Name="Pressed">
                                    <storyboard>
                                        <coloranimation BeginTime="0" Storyboard.TargetName="arrow" Storyboard.TargetProperty="(Path.Stroke).(SolidColorBrush.Color)" To="#FF003366"/>
                                        <coloranimationusingkeyframes BeginTime="00:00:00" Duration="00:00:00.0010000" Storyboard.TargetName="MouseOverBackground" Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)">
                                            <splinecolorkeyframe KeyTime="00:00:00" Value="Green"/>
                                        </ColorAnimationUsingKeyFrames>
                                    </Storyboard>
                                </vsm:VisualState>
                                <vsm:VisualState x:Name="Disabled">
                                    <Storyboard/>
                                </vsm:VisualState>
                            </vsm:VisualStateGroup>
                            <vsm:VisualStateGroup x:Name="FocusStates">
                                <vsm:VisualState x:Name="Focused">
                                    <storyboard>
                                        <objectanimationusingkeyframes Duration="0" Storyboard.TargetName="FocusVisualElement" Storyboard.TargetProperty="Visibility">
                                            <discreteobjectkeyframe KeyTime="0" Value="Visible"/>
                                        </ObjectAnimationUsingKeyFrames>
                                        <doubleanimationusingkeyframes BeginTime="00:00:00" Duration="00:00:00.0010000" Storyboard.TargetName="FocusVisualElement" Storyboard.TargetProperty="(UIElement.Opacity)">
                                            <splinedoublekeyframe KeyTime="00:00:00" Value="0.385"/>
                                        </DoubleAnimationUsingKeyFrames>
                                    </Storyboard>
                                </vsm:VisualState>
                                <vsm:VisualState x:Name="Unfocused"/>
                            </vsm:VisualStateGroup>
                        </vsm:VisualStateManager.VisualStateGroups>
                        <border x:Name="background" Background="{TemplateBinding Background}" CornerRadius="1,1,1,1">
                            <grid>
                                <border Height="Auto" Margin="0,0,0,0" x:Name="ExpandedBackground" VerticalAlignment="Stretch" Opacity="0" Background="#FFBADDE9" BorderBrush="{TemplateBinding BorderBrush}" BorderThickness="{TemplateBinding BorderThickness}" CornerRadius="1,1,1,1"/>
                                <border Height="Auto" Margin="0,0,0,0" x:Name="MouseOverBackground" VerticalAlignment="Stretch" Opacity="0" Background="#FFBDBDBD" BorderBrush="{TemplateBinding BorderBrush}" BorderThickness="{TemplateBinding BorderThickness}" CornerRadius="1,1,1,1"/>
                                <grid Background="Transparent">
                                    <Grid.ColumnDefinitions>
                                        <columndefinition Width="Auto" x:Name="cd0"/>
                                        <columndefinition Width="Auto" x:Name="cd1"/>
                                    </Grid.ColumnDefinitions>
                                    <Grid.RowDefinitions>
                                        <rowdefinition Height="Auto" x:Name="rd0"/>
                                        <rowdefinition Height="Auto" x:Name="rd1"/>
                                    </Grid.RowDefinitions>
                                    <grid Height="19" HorizontalAlignment="Center" x:Name="icon" VerticalAlignment="Center" Width="19" RenderTransformOrigin="0.5,0.5" Grid.Column="0" Grid.Row="0">
                                        <Grid.RenderTransform>
                                            <transformgroup>
                                                <ScaleTransform/>
                                                <SkewTransform/>
                                                <rotatetransform Angle="-90"/>
                                                <TranslateTransform/>
                                            </TransformGroup>
                                        </Grid.RenderTransform>
                                        <path
                                            Height="Auto"
                                            HorizontalAlignment="Center"
                                            Margin="0,0,0,0" x:Name="arrow"
                                            VerticalAlignment="Center"
                                            Width="Auto"
                                            RenderTransformOrigin="0.5,0.5"
                                            Stroke="#666"
                                            StrokeThickness="2"
                                            Data="M 1,1.5 L 4.5,5 L 8,1.5">
                                            <Path.RenderTransform>
                                                <transformgroup>
                                                    <ScaleTransform/>
                                                    <SkewTransform/>
                                                    <RotateTransform/>
                                                    <TranslateTransform/>
                                                </TransformGroup>
                                            </Path.RenderTransform>
                                        </Path>
                                    </Grid>
                                    <layoutToolkit:LayoutTransformer
                                        FontFamily="{TemplateBinding FontFamily}"
                                        FontSize="{TemplateBinding FontSize}"
                                        FontStretch="{TemplateBinding FontStretch}"
                                        FontStyle="{TemplateBinding FontStyle}"
                                        FontWeight="{TemplateBinding FontWeight}"
                                        Foreground="{TemplateBinding Foreground}"
                                        HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                                        Margin="6,6,6,0"
                                        x:Name="header"
                                        Grid.Column="1"
                                        Grid.Row="0"
                                        Grid.RowSpan="1"
                                        Content="{TemplateBinding Content}"
                                        ContentTemplate="{TemplateBinding ContentTemplate}"/>
                                </Grid>
                            </Grid>
                        </Border>
                        <rectangle x:Name="FocusVisualElement" IsHitTestVisible="false" Visibility="Collapsed" RadiusX="1" RadiusY="1" Stroke="#FF6DBDD1" StrokeThickness="1"/>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
```

Note that in the Visual States I changed not only the Color in the animation but also the Opacity to 1 which in the original Style is set to some lower value. Also note that you will need to import these namespaces:

```xml
xmlns:layoutPrimitivesToolkit="clr-namespace:System.Windows.Controls.Primitives;assembly=System.Windows.Controls.Layout.Toolkit"
xmlns:vsm="clr-namespace:System.Windows;assembly=System.Windows"
```

And than you can just easily apply the style like this:

```xml
<layoutToolkit:Accordion AccordionButtonStyle="{StaticResource MyAccButtonStyle}">
...your items here...
</layoutToolkit:Accordion>
```
