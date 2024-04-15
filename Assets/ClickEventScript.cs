// Copyright © 2024 Daylon Wilkins
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using UnityEngine;
using UnityEngine.EventSystems;

public class ClickEventScript : MonoBehaviour, IPointerClickHandler {
    public string EventName;

    public void OnPointerClick(PointerEventData eventData) {
        SystemScript.System.ClickEvents.Enqueue(EventName);
    }
}
