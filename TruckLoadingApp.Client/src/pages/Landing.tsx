import { FC, useState, useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';
import { motion, useAnimation, AnimatePresence } from 'framer-motion';
import { useInView } from 'react-intersection-observer';
import { 
  FaTruck, 
  FaUserClock, 
  FaChartLine, 
  FaShieldAlt, 
  FaBars, 
  FaTimes,
  FaAngleDown
} from 'react-icons/fa';
import '../css/Landing.css';

const Landing: FC = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [scrollPosition, setScrollPosition] = useState(0);
  const heroControls = useAnimation();
  const featuresControls = useAnimation();
  
  // References for scroll animations
  const [featuresRef, featuresInView] = useInView({ threshold: 0.1, triggerOnce: true });
  const [ctaRef, ctaInView] = useInView({ threshold: 0.1, triggerOnce: true });
  
  // Detect scroll position for navbar transparency
  useEffect(() => {
    const handleScroll = () => {
      const position = window.pageYOffset;
      setScrollPosition(position);
    };

    window.addEventListener('scroll', handleScroll);
    return () => {
      window.removeEventListener('scroll', handleScroll);
    };
  }, []);
  
  // Trigger animations when elements come into view
  useEffect(() => {
    if (featuresInView) {
      featuresControls.start('visible');
    }
  }, [featuresControls, featuresInView]);

  //features function
  const features = [
    { 
      icon: <FaTruck className="w-6 h-6" />,
      title: "Load Optimization",
      description: "Smart algorithms to maximize truck space utilization and optimize loading arrangements."
    },
    {
      icon: <FaUserClock className="w-6 h-6" />,
      title: "Driver Management",
      description: "Comprehensive driver tracking, scheduling, and compliance monitoring system."
    },
    {
      icon: <FaChartLine className="w-6 h-6" />,
      title: "Analytics Dashboard",
      description: "Real-time insights into loading operations, efficiency metrics, and performance tracking."
    },
    {
      icon: <FaShieldAlt className="w-6 h-6" />,
      title: "Compliance Assurance",
      description: "Automated validation of driver schedules and rest periods to ensure regulatory compliance."
    }
  ];

  // Advanced animation variants
  const fadeInUp = {
    hidden: { y: 30, opacity: 0 },
    visible: { 
      y: 0, 
      opacity: 1,
      transition: { 
        type: "spring", 
        stiffness: 100, 
        damping: 10 
      }
    }
  };

  const staggerContainer = {
    hidden: { opacity: 0 },
    visible: {
      opacity: 1,
      transition: {
        staggerChildren: 0.2
      }
    }
  };
  
  const logoAnimation = {
    hidden: { rotate: -5, scale: 0.9, opacity: 0 },
    visible: { 
      rotate: 0, 
      scale: 1, 
      opacity: 1,
      transition: { 
        type: "spring", 
        stiffness: 200, 
        damping: 20 
      }
    }
  };

  return (
    <div className="min-h-screen bg-white">
      {/* Background elements */}
      <div className="hero-background"></div>
      
      {/* Navigation - Enhanced with scroll effect */}
      <nav className={`sticky top-0 z-50 ${scrollPosition > 50 ? 'glass-nav shadow-md' : 'bg-transparent'} transition-all duration-300`}>
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex">
              <motion.div 
                initial="hidden"
                animate="visible"
                variants={logoAnimation}
                className="flex-shrink-0 flex items-center"
              >
                <FaTruck className="h-8 w-8 logo-float" style={{color:"#002244"}}/>
                <span className="ml-2 text-xl font-bold text-gray-900">TruckLoadingApp</span>
              </motion.div>
            </div>
            <div className="hidden sm:ml-6 sm:flex sm:items-center sm:space-x-8">
              {["Login", "Register as Shipper", "Register as Trucker", "Register as Company"].map((item, i) => (
                <motion.div 
                  key={i}
                  initial={{ y: -20, opacity: 0 }}
                  animate={{ y: 0, opacity: 1 }}
                  transition={{ delay: 0.1 * i, duration: 0.5 }}
                >
                  <Link 
                    to={`/${item.toLowerCase().replace(/ as /g, '/').replace(/ /g, '')}`} 
                    className="text-gray-500 hover:text-gray-900 px-3 py-2 rounded-md text-sm font-medium"
                  >
                    {item}
                  </Link>
                </motion.div>
              ))}
            </div>
            <div className="flex items-center sm:hidden">
              <motion.button
                whileTap={{ scale: 0.95 }}
                onClick={() => setIsMenuOpen(!isMenuOpen)}
                className="inline-flex items-center justify-center p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-blue-500"
              >
                {isMenuOpen ? (
                  <FaTimes className="block h-6 w-6" />
                ) : (
                  <FaBars className="block h-6 w-6" />
                )}
              </motion.button>
            </div>
          </div>
        </div>
        
        {/* Mobile menu with improved animations */}
        <AnimatePresence>
          {isMenuOpen && (
            <motion.div 
              className="sm:hidden"
              initial={{ height: 0, opacity: 0 }}
              animate={{ height: 'auto', opacity: 1 }}
              exit={{ height: 0, opacity: 0 }}
              transition={{ duration: 0.3 }}
            >
              <div className="pt-2 pb-3 space-y-1">
                {["Login", "Register as Shipper", "Register as Trucker", "Register as Company"].map((item, i) => (
                  <motion.div
                    key={i}
                    initial={{ x: -20, opacity: 0 }}
                    animate={{ x: 0, opacity: 1 }}
                    transition={{ delay: 0.05 * i }}
                  >
                    <Link
                      to={`/${item.toLowerCase().replace(/ as /g, '/').replace(/ /g, '')}`}
                      className="block px-3 py-2 rounded-md text-base font-medium text-gray-700 hover:text-gray-900 hover:bg-gray-50"
                      onClick={() => setIsMenuOpen(false)}
                    >
                      {item}
                    </Link>
                  </motion.div>
                ))}
              </div>
            </motion.div>
          )}
        </AnimatePresence>
      </nav>

      {/* Hero Section - Enhanced with parallax and advanced animations */}
      <div className="relative overflow-hidden hero-gradient">
        <div className="max-w-7xl mx-auto">
          <div className="relative z-10 pb-8 sm:pb-16 md:pb-20 lg:max-w-2xl lg:w-full lg:pb-28 xl:pb-32">
            <motion.main
              initial="hidden"
              animate="visible"
              variants={staggerContainer}
              className="mt-10 mx-auto max-w-7xl px-4 sm:mt-12 sm:px-6 md:mt-16 lg:mt-20 lg:px-8 xl:mt-28"
            >
              <motion.div variants={fadeInUp} className="sm:text-center lg:text-left">
                <h1 className="text-4xl tracking-tight font-extrabold  sm:text-5xl md:text-6xl">
                  <motion.span 
                    className="block animated-gradient-text secondary-gradient-text"
                    variants={fadeInUp}
                  >
                    Optimize Your
                  </motion.span>
                  <motion.span 
                    className="block animated-gradient-text primary-gradient-text"
                    variants={fadeInUp}
                  >
                    Truck Loading Operations
                  </motion.span>
                </h1>
                <motion.p 
                  variants={fadeInUp}
                  className="mt-3 text-base text-white sm:mt-5 sm:text-lg sm:max-w-xl sm:mx-auto md:mt-5 md:text-xl lg:mx-0"
                >
                  Streamline your logistics with our advanced truck loading management system. 
                  Optimize loads, manage drivers, and ensure compliance all in one place.
                </motion.p>
                <div className="mt-5 sm:mt-8 sm:flex sm:justify-center lg:justify-start">
                  <motion.div 
                    variants={fadeInUp}
                    className="rounded-md shadow"
                    whileHover={{ scale: 1.05 }}
                    whileTap={{ scale: 0.95 }}
                  >
                    <Link
                      to="/login"
                      className="w-full flex items-center justify-center px-8 py-3 border border-transparent text-base font-medium rounded-md text-white btn-primary md:py-4 md:text-lg md:px-10"
                    >
                      Get Started
                    </Link>
                  </motion.div>
                  <motion.div 
                    variants={fadeInUp}
                    className="mt-3 sm:mt-0 sm:ml-3"
                    whileHover={{ scale: 1.05 }}
                    whileTap={{ scale: 0.95 }}
                  >
                    <Link
                      to="/register/company"
                      className="w-full flex items-center justify-center px-8 py-3 border border-transparent text-base font-medium rounded-md text-blue-700 btn-secondary md:py-4 md:text-lg md:px-10"
                    >
                      Register Company
                    </Link>
                  </motion.div>
                </div>
              </motion.div>
            </motion.main>
          </div>
        </div>
        
        {/* Animated scroll indicator */}
        <motion.div 
          className="absolute bottom-10 left-1/2 transform -translate-x-1/2 hidden sm:block"
          animate={{ y: [0, 10, 0] }}
          transition={{ repeat: Infinity, duration: 1.5 }}
        >
          <FaAngleDown className="text-blue-500 h-6 w-6" />
        </motion.div>
      </div>

      {/* Features Section - Enhanced with stagger animations and hover effects */}
      <motion.div 
        ref={featuresRef}
        initial="hidden"
        animate={featuresControls}
        variants={staggerContainer}
        className="py-12 bg-white"
      >
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8" >
          <motion.div 
            className="lg:text-center" 
            variants={fadeInUp}
          >
            <h2 className="text-base  font-semibold tracking-wide uppercase" style={{color:"#002244"}}>Features</h2>
            <p className="mt-2 text-3xl leading-8 font-extrabold tracking-tight text-gray-900 sm:text-4xl">
              Better Way To Handle Non Revenue Miles
            </p>
            <p className="mt-4 max-w-2xl text-xl text-gray-500 lg:mx-auto">
              Our comprehensive solution provides everything you need to optimize your truck loading operations.
            </p>
          </motion.div>

          <div className="mt-10">
            <div className="grid grid-cols-1 gap-10 sm:grid-cols-2 lg:grid-cols-4">
              {features.map((feature, index) => (
                <motion.div
                  key={index}
                  variants={fadeInUp}
                  className="pt-6"
                >
                  <div className="feature-card rounded-xl px-6 pb-8">
                    <div className="-mt-6 content">
                      <div>
                        <span className="feature-icon">
                          {feature.icon}
                        </span>
                      </div>
                      <h3 className="mt-8 text-lg font-medium text-gray-900 tracking-tight">{feature.title}</h3>
                      <p className="mt-5 text-base text-gray-500">{feature.description}</p>
                    </div>
                  </div>
                </motion.div>
              ))}
            </div>
          </div>
        </div>
      </motion.div>

      {/* CTA Section - Enhanced with gradient and particle background */}
      <motion.div 
        ref={ctaRef}
        initial={{ opacity: 0 }}
        whileInView={{ opacity: 1 }}
        viewport={{ once: true }}
        className="cta-gradient relative overflow-hidden"
      >
        <div className="max-w-7xl mx-auto py-12 px-4 sm:px-6 lg:py-16 lg:px-8 lg:flex lg:items-center lg:justify-between relative z-10">
          <motion.h2 
            initial={{ x: -30, opacity: 0 }}
            whileInView={{ x: 0, opacity: 1 }}
            viewport={{ once: true }}
            className="text-3xl font-extrabold tracking-tight text-white sm:text-4xl"
          >
            <span className="block">Ready to get started?</span>
            <span className="block text-blue-200">Start optimizing your operations today.</span>
          </motion.h2>
          <motion.div 
            initial={{ x: 30, opacity: 0 }}
            whileInView={{ x: 0, opacity: 1 }}
            viewport={{ once: true }}
            transition={{ delay: 0.2 }}
            className="mt-8 flex lg:mt-0 lg:flex-shrink-0"
          >
            <div className="inline-flex rounded-md shadow">
              <motion.div
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
              >
                <Link
                  to="/register/shipper"
                  className="inline-flex items-center justify-center px-5 py-3 border border-transparent text-base font-medium rounded-md text-blue-600 bg-white hover:bg-blue-50"
                >
                  Sign up now
                </Link>
              </motion.div>
            </div>
          </motion.div>
        </div>
      </motion.div>

      {/* Footer - Enhanced with subtle animation */}
      <motion.footer 
        initial={{ opacity: 0 }}
        whileInView={{ opacity: 1 }}
        viewport={{ once: true }}
        className="bg-gray-50"
      >
        <div className="max-w-7xl mx-auto py-12 px-4 sm:px-6 lg:px-8">
          <div className="text-center">
            <p className="text-base text-gray-400">&copy; 2024 TruckLoadingApp. All rights reserved.</p>
          </div>
        </div>
      </motion.footer>
    </div>
  );
};

export default Landing;