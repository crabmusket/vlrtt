test(StateMachine);

function StateMachineTests::before() {
   $SM::leaveNotReady_called = false;
   $SM::enterReady_called = false;
   $SM::onGo_called = false;

   new ScriptObject(SM) {
      class = StateMachine;
      state = notready;
   };

   expect(SM.state).toEqual(notready);

   expect($SM::onGo_called).toBe(false);
   expect($SM::leaveNotReady_called).toBe(false);
   expect($SM::enterReady_called).toBe(false);
}

function StateMachineTests::after() {
   SM.delete();
}

function SM::leaveNotReady() { $SM::leaveNotReady_called = true; }
function SM::enterReady() { $SM::enterReady_called = true; }
function SM::onGo() { $SM::onGo_called = true; }

function StateMachineShould::have_an_onEvent_method() {
   expect(SM.isMethod(onEvent)).toBe(true);
}

function StateMachineShould::follow_a_simple_transition() {
   SM.transition[notready, go] = ready;

   SM.onEvent(go);
   expect(SM.state).toEqual(ready);

   expect($SM::onGo_called).toBe(true);
   expect($SM::leaveNotReady_called).toBe(true);
   expect($SM::enterReady_called).toBe(true);
}

function StateMachineShould::follow_a_wildcard_transition() {
   SM.transition[notready, _] = ready;

   SM.onEvent(blah);
   expect(SM.state).toEqual(ready);

   expect($SM::onGo_called).toBe(false);
   expect($SM::leaveNotReady_called).toBe(true);
   expect($SM::enterReady_called).toBe(true);
}

function StateMachineShould::respond_to_a_wildcard_event() {
   SM.transition[_, go] = ready;

   SM.onEvent(go);
   expect(SM.state).toEqual(ready);

   expect($SM::onGo_called).toBe(true);
   expect($SM::leaveNotReady_called).toBe(true);
   expect($SM::enterReady_called).toBe(true);
}

function StateMachineShould::not_transition_to_an_empty_state() {
   SM.transition[notready, go] = "";

   SM.onEvent(go);
   expect(SM.state).toEqual(notready);

   expect($SM::onGo_called).toBe(true);
   expect($SM::leaveNotReady_called).toBe(false);
   expect($SM::enterReady_called).toBe(false);
}

function StateMachineShould::not_allow_an_event_called_event() {
   SM.transition[notready, event] = ready;

   SM.onEvent(event);
   expect(SM.state).toEqual(notready);

   expect($SM::onGo_called).toBe(false);
   expect($SM::leaveNotReady_called).toBe(false);
   expect($SM::enterReady_called).toBe(false);
}

Tasman.runAll();
Tasman.cleanUp();
