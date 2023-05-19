using System;
using System.Linq;
using System.Reflection;
using Plugins.MonoCache.System;
using UnityEngine;

namespace Plugins.MonoCache
{
    public class ExceptionsChecker
    {
        private const BindingFlags MethodFlags = BindingFlags.Public 
                                                 | BindingFlags.NonPublic 
                                                 | BindingFlags.Instance 
                                                 | BindingFlags.DeclaredOnly;
        
        public void CheckForExceptions()
        {
            var subclassTypes = Assembly
                .GetAssembly(typeof(MonoCache))
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(MonoCache)));
            
            foreach (var type in subclassTypes)
            {
                MethodInfo[] methods = type.GetMethods(MethodFlags);
                
                foreach (MethodInfo method in methods)
                {
                    switch (method.Name)
                    {
                        case GlobalUpdate.OnEnableMethodName:
                            DebugSelector(type, "OnEnabled()");
                            break;
                        case GlobalUpdate.OnDisableMethodName:
                            DebugSelector(type, "OnDisabled()");
                            break;
                        case GlobalUpdate.UpdateMethodName:
                            DebugUpdate(method, type, "UpdateCached()");
                            break;
                        case GlobalUpdate.FixedUpdateMethodName:
                            DebugUpdate(method, type, "FixedUpdateCached()");
                            break;
                        case GlobalUpdate.LateUpdateMethodName:
                            DebugUpdate(method, type, "LateUpdateCached()");
                            break;
                    }
                }
            }
        }

        private void DebugUpdate(MethodInfo method, Type type, string recommendedMethod) => 
            Debug.LogWarning(GetWarningBaseText(method.Name, recommendedMethod, type.Name));

        private void DebugSelector(Type type, string recommendedMethod) =>
            Debug.LogException(new Exception($"{GetExceptionBaseText(GlobalUpdate.OnEnableMethodName, type.Name)}" +
                                             $"{ColoredText.GetColoredText(ColoredText.BlueColor, "protected override void")} " +
                                             $"{ColoredText.GetColoredText(ColoredText.OrangeColor, recommendedMethod)}"));

        private string GetExceptionBaseText(string methodName, string className)
        {
            string classNameColored = ColoredText.GetColoredText(ColoredText.RedColor, className);
            string monoCacheNameColored = ColoredText.GetColoredText(ColoredText.OrangeColor, nameof(MonoCache));
            string methodNameColored = ColoredText.GetColoredText(ColoredText.RedColor, methodName);
            string baseTextColored = ColoredText.GetColoredText(ColoredText.WhiteColor,
                $"can't be implemented in subclass {classNameColored} of {monoCacheNameColored}. Use ");
            
            return $"{methodNameColored} {baseTextColored}";
        }

        private string GetWarningBaseText(string methodName, string recommendedMethod, string className)
        {
            string coloredClass = ColoredText.GetColoredText(ColoredText.OrangeColor, className);
            string monoCacheNameColored = ColoredText.GetColoredText(ColoredText.OrangeColor, nameof(MonoCache));
            string coloredMethod = ColoredText.GetColoredText(ColoredText.OrangeColor, methodName);
            
            string coloredRecommendedMethod =
                ColoredText.GetColoredText(ColoredText.BlueColor, "protected override void ") + 
                ColoredText.GetColoredText(ColoredText.OrangeColor, recommendedMethod);
            
            string coloredBaseText = ColoredText.GetColoredText(ColoredText.WhiteColor, 
                $"It is recommended to replace {coloredMethod} method with {coloredRecommendedMethod} " +
                $"in subclass {coloredClass} of {monoCacheNameColored}");
            
            return coloredBaseText;
        }
    }
}