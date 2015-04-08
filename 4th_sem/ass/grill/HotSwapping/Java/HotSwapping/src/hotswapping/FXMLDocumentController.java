/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package hotswapping;

import com.sun.jdi.connect.IllegalConnectorArgumentsException;
import hotswapping.swappingClass.SwappingClass;
import java.io.IOException;
import java.net.URL;
import java.util.ResourceBundle;
import java.util.logging.Level;
import java.util.logging.Logger;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Label;
import javafx.scene.control.TextArea;
import javassist.CannotCompileException;
import javassist.ClassPool;
import javassist.CtClass;
import javassist.CtMethod;
import javassist.NotFoundException;
import javassist.util.HotSwapper;

/**
 *
 * @author Flo
 */
public class FXMLDocumentController {
    
    private SwappingClass swappingClass;
    private HotSwapper swap;
    
    @FXML
    private Label javassistError;
    @FXML
    private TextArea javassistTextArea; 
    
    public FXMLDocumentController(){
        this.swappingClass = new SwappingClass();
        try {
            this.swap = new HotSwapper("8000");
        } catch (IOException ex) {
            Logger.getLogger(FXMLDocumentController.class.getName()).log(Level.SEVERE, null, ex);
        } catch (IllegalConnectorArgumentsException ex) {
            Logger.getLogger(FXMLDocumentController.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    @FXML
    private void handleJavassistButton(ActionEvent event) throws IllegalConnectorArgumentsException {
        if(!javassistTextArea.getText().equals("") && this.swap != null){
            javassistError.setVisible(false);
            ClassPool pool = ClassPool.getDefault();
            try {
                
                CtClass target = pool.get("hotswapping.swappingClass.SwappingClass");
                target.defrost();
                CtMethod targetMethod = target.getDeclaredMethod("swappingMethod");
                targetMethod.setBody(javassistTextArea.getText());
                
                swap.reload("hotswapping.swappingClass.SwappingClass", target.toBytecode());
                //CtClass base = pool.get("hotswapping.additionalClasses.ConsoleWriter");
                //target.setSuperclass(base);
                //  translates the CtClass object into a class file and writes it on a local disk
                //target.writeFile();
            } catch (CannotCompileException ex) {
                Logger.getLogger(FXMLDocumentController.class.getName()).log(Level.SEVERE, null, ex);
            } catch (NotFoundException ex) {
                Logger.getLogger(FXMLDocumentController.class.getName()).log(Level.SEVERE, null, ex);
            } catch (SecurityException ex) {
                Logger.getLogger(FXMLDocumentController.class.getName()).log(Level.SEVERE, null, ex);
            } catch (IllegalArgumentException ex) {
                Logger.getLogger(FXMLDocumentController.class.getName()).log(Level.SEVERE, null, ex);
            } catch (IOException ex) {
                Logger.getLogger(FXMLDocumentController.class.getName()).log(Level.SEVERE, null, ex);
            }
        } else{
            javassistError.setVisible(true);
        }
    }

    @FXML
    private void executeCode(ActionEvent event) {
        swappingClass.swappingMethod();
    }
}
