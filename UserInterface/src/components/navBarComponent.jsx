import React from 'react';
import '../css/nav.css';

class NavBarComponent extends React.Component {

    componentDidMount() {
        require('../scripts/navHandler'); 
    }

    render() {
        return (
            <nav id="nav" className="navStyle">
                <button id="saveButton" className="nav-button">Save</button>
                <button id="loadButton" className="nav-button">Load</button>
                <button id="newButton" className="nav-button">New</button>
            </nav>
        );
    }
}

export default NavBarComponent;