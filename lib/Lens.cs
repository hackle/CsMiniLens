using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CSharpLens
{
    public class LensInner<A, B> {
        private Expression<Func<A, B>> exp;

        public LensInner(Expression<Func<A, B>> exp)
        {
            this.exp = exp;
        }

        public B View(A a) {
            return (B) ViewRec(exp.Body, a);
        }

        static MethodInfo ShallowCopy = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
        static object ViewRec(Expression exp, object o) {
            if (exp.NodeType == ExpressionType.Parameter) {
                return o;
            }
            else if (exp.NodeType == ExpressionType.TypeAs) {
                var caster = exp as UnaryExpression;
                var obj = ViewRec(caster.Operand, o);
                if (obj != null && caster.Type.IsAssignableFrom(obj.GetType())) {
                    return obj;
                } else {
                    return null;
                }
            }
            else if (exp.NodeType == ExpressionType.MemberAccess) {
                var memberAccessExp = exp as MemberExpression;
                var parent = ViewRec(memberAccessExp.Expression, o);
                if (parent == null) return null;

                return ((memberAccessExp.Member as PropertyInfo)?.GetValue(parent)
                            ?? (memberAccessExp.Member as FieldInfo)?.GetValue(parent));
            }
                            
            throw new InvalidOperationException($"Expression type not supported");
        }

        static Func<object, object> SetRec(Expression exp, Func<object, object> setter) {
            if (exp.NodeType == ExpressionType.Parameter) {
                return setter;
            }
            else if (exp.NodeType == ExpressionType.TypeAs) {
                var caster = exp as UnaryExpression;

                Func<object, object> newSetter = parent => {
                    if (parent != null && caster.Type.IsAssignableFrom(parent.GetType())) {
                        return setter(parent);
                    } 
                    
                    return parent;
                };

                return SetRec(caster.Operand, newSetter);
            }
            else if (exp.NodeType == ExpressionType.MemberAccess) {
                var memberAccessExp = exp as MemberExpression;

                var propInfo = memberAccessExp.Member as PropertyInfo;
                var fieldInfo = memberAccessExp.Member as FieldInfo;
                Func<object, object> nextSetter = parent => {
                    if (parent != null) {
                        parent = ShallowCopy.Invoke(parent, null);

                        var v = propInfo?.GetValue(parent) ?? fieldInfo?.GetValue(parent);
                        
                        propInfo?.SetValue(parent, setter(v));
                        fieldInfo?.SetValue(parent, setter(v));
                    }

                    return parent;
                };

                return SetRec(memberAccessExp.Expression, nextSetter);
            }

            throw new InvalidOperationException($"Expression type not supported");
        }

        public A Set(A a, B b)
        {
            return (A) SetRec(exp.Body, _ => b)(a);
        }
    }
    public class Lens
    {
        public static LensInner<A, B> For<A, B>(Expression<Func<A, B>> exp) {
            return new LensInner<A, B>(exp);
        }
    }
}
