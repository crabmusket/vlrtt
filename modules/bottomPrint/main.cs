new ScriptObject(BottomPrint);

exec("./bottomPrint.gui");

function BottomPrint::onStart(%this) {
   PlayGui.add(BottomPrintDlg);
}
