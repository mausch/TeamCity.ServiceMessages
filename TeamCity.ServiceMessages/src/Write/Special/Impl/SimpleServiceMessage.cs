/*
 * Copyright 2007-2011 JetBrains s.r.o.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;

namespace JetBrains.TeamCity.ServiceMessages.Write.Special.Impl
{
  internal class SimpleServiceMessage : IServiceMessage, IEnumerable<KeyValuePair<string, string>> 
  {
    private readonly string myName;    
    private readonly Dictionary<string, string> myArguments = new Dictionary<string, string>();

    public SimpleServiceMessage(string name, Dictionary<string, string> arguments) : this(name)
    {      
      foreach (var arg in arguments)
      {
        Add(arg.Key, arg.Value);
      }      
    }

    public SimpleServiceMessage(string name) 
    {
      myName = name;
    }

    public void Add(string key, string value)
    {
      myArguments[key] = value;
    }

    public void AddRange(IEnumerable<KeyValuePair<string, string>> values)
    {
      foreach (var e in values)
      {
        myArguments[e.Key] = e.Value;
      }      
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
      return myArguments.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public string Name
    {
      get { return myName; }
    }

    public string DefaultValue
    {
      get { return null; }
    }

    public IEnumerable<string> Keys
    {
      get { return myArguments.Keys; }
    }

    public string GetValue(string key)
    {
      string s;
      return myArguments.TryGetValue(key, out s) ? s : null;
    }
  }
}