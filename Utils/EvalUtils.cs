using UnityEngine;

public static class EvalUtils
{
    public const string TRUE = "TRUE";
    public const string FALSE = "FALSE";

    static string debugString = null;
    static bool debugging = false;
    static bool originator = false;

    static public bool ParseCondition(string condition, System.Func<string, string> getMethod)
    {
        if (condition == null)
        {
            return false;
        }

        bool result = false;
        string originalCondition = condition;

        if (originator == false)
        {
            originator = true;
            debugString = "";
        }

        int startBI = condition.IndexOf('(');
        int endBI = condition.LastIndexOf(')');
        if ((startBI == -1) != (endBI == -1) || endBI < startBI)
        {
            Debug.LogError("Condition '" + condition + "' has weird brackets.");
            return false;
        }
        else if (startBI != -1)
        {
            string bracketCondition = condition.Substring(startBI+1, endBI - startBI - 1).Trim();
            bool bracketResult = ParseCondition(bracketCondition, getMethod);
            debugString += "\n  Brackets (" + bracketCondition + ") WAS " + bracketResult.ToString().ToUpper();
            condition = condition.Substring(0, startBI) + " " + bracketResult.ToString() + " " + condition.Substring(endBI+1);
            debugString += "\n  Condition is now '" + condition + "'.";
        }

        int andI = condition.IndexOf("&&");
        int orI = condition.IndexOf("||");

        if (andI == -1 && orI == -1)
        {
            result = ParseSubCondition(condition, getMethod);
            debugString += "\n  Simple (" + condition.Trim() + ") WAS " + result.ToString().ToUpper();
        }
        else
        {
            bool isAnd = orI == -1 || (andI != -1 && andI < orI);

            int matchIndex = isAnd ? andI : orI;

            string left = condition.Substring(0, matchIndex).Trim();
            string right = condition.Substring(matchIndex+2).Trim();

            bool leftResult = ParseCondition(left, getMethod);
            bool rightResult = ParseCondition(right, getMethod);

            if (isAnd)
            {
                result = leftResult && rightResult;  
            }
            else
            {
                result = leftResult || rightResult;
            }

            debugString += "\n  Compound (" + left + (isAnd ? " && " : " || ") + right + ") WAS " + result.ToString().ToUpper();
        }

        if (originator)
        {
            Debug.Log("Condition '" + originalCondition + "' evalutes to " + result.ToString().ToLower() + debugString);
            originator = false;
        }

        return result;
    }

    static bool ParseSubCondition(string condition, System.Func<string, string> getMethod)
    {
        if (ParseToken(condition) == TRUE)
            return true;

        if (ParseToken(condition) == FALSE)
            return false;
        

        if (getMethod == null)
        {
            getMethod = BlankGetMethod;
        }

        string key = condition.Trim();
        if (string.IsNullOrEmpty(key))
            return false;

        string value = TRUE;

        int lt = condition.IndexOf('<');
        int gt = condition.IndexOf('>');
        if (lt > 0 || gt > 0)
        {
            int index = Mathf.Max(lt, gt);
            bool orEqual = key[index + 1] == '=';
            value = ParseToken(key.Substring(index + 1));
            key = ParseToken(key.Substring(0, index));
            if (orEqual)
            {
                key = key.Substring(1);
            }
            var intLeft = 0;
            var intRight = 0;
            int.TryParse(getMethod(key), out intLeft);
            int.TryParse(value, out intRight);
            if (orEqual)
            {
                return gt > lt ? intLeft >= intRight : intLeft <= intRight;
            }
            else
            {
                return gt > lt ? intLeft > intRight : intLeft < intRight;
            }
        }

        bool compareTrue = true;

        int neq = condition.IndexOf("!=");
        if (neq > 0)
        {
            compareTrue = false;
            value = ParseToken(key.Substring(neq + 2));
            key = ParseToken(key.Substring(0, neq - 1));
        }
        else
        {
            int eq = condition.IndexOf("==");
            if (eq > 0)
            {
                value = ParseToken(key.Substring(eq + 2));
                key = ParseToken(key.Substring(0, eq - 1));
            }
            else
            {
                if (key[0] == '!')
                {
                    key = key.Substring(1);
                    compareTrue = false;
                }
            }
        }

        string dataValue;

        if (key == TRUE || key == FALSE)
        {
            dataValue = key;
        }
        else
        {
            dataValue = ParseToken(getMethod(key));
        }

        bool result = dataValue == value == compareTrue;
        return result;
    }
        
    static public string ParseToken(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        name = name.ToUpper().Trim();
        var key = "";
        for (var i=0; i<name.Length; i++)
        {
            var c = name[i];
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z'))
            {
                key += c;
            }
            else
            {
                key += "_";
            }
        }
        return key;
    }

    public static void RunTests()
    {
        RunTest("1 == 1", true);
        RunTest("1 == 2", false);
        RunTest("A == A && B == B", true);
        RunTest("A == B && B == A", false);
        RunTest("2 < 3", true);
        RunTest("2 > 3", false);
        RunTest("3 > 2", true);
        RunTest("3 < 2", false);
        RunTest("(1 == 1) != (1 == 2)", true);
        RunTest("A != B", true);
        RunTest("A != A", false);
        RunTest("SimpleValue", true);
        RunTest(TRUE, true);
        RunTest(FALSE, false);
        RunTest(null, false);
        RunTest("", false);
    }

    static void RunTest(string condition, bool expectedResult)
    {
        var result = ParseCondition(condition, null);
        if (result == expectedResult)
        {
            Debug.LogFormat("Test '{0}' passed", condition);
        }
        else
        {
            Debug.LogErrorFormat("Test '{0}' failed \n {1}", condition, debugString);
        }
    }

    static string BlankGetMethod(string key)
    {
        return key;
    }
}